using MySql.Data.MySqlClient;
using Dapper; 
using SportsEquipment.Interfaces;
using SportsEquipment.Model;
using SportsEquipment.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;
using Mysqlx.Crud;
using System.Data.Common;
using System.Data;
using System.Security.Cryptography;
using System.Text;


namespace SportsEquipment.Services
{
    public class ClientService : IClientService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public ClientService(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        // Create a new connection
        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // Get all products with optional pagination and search
        public async Task<IEnumerable<products>> GetAllProductsAsync(int? pageIndex, int? pageSize, string? searchValue)
        {
            using var connection = CreateConnection();
            string sql = "CALL sportsregistration.sp_product(@pageIndex, @pageSize, @searchValue)";
            var parameters = new { pageIndex, pageSize, searchValue = string.IsNullOrEmpty(searchValue) ? null : searchValue };
            return await connection.QueryAsync<products>(sql, parameters);
        }


        // Add a product
        public async Task<int> AddProductAsync(products product)
        {
            using var connection = CreateConnection();
            string sql = @"
                INSERT INTO products (product_id, product_name, image_url, status, description, category_id) 
                VALUES (@product_id, @product_name, @image_url, @status, @description, @category_id)";
            return await connection.ExecuteAsync(sql, product);
        }

        // Delete a product by ID
        public async Task<int> DeleteProductAsync(int product_id)
        {
            using var connection = CreateConnection();
            string sql = "DELETE FROM products WHERE product_id = @product_id";
            return await connection.ExecuteAsync(sql, new { product_id });
        }




        /// <summary>
        /// customer service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>


        // Get client data by ID
        public async Task<ClientModel> GetClientDataById(int id)
        {
            using var connection = CreateConnection();
            string sql = @"
                SELECT 
                    r.FirstName, r.LastName, r.Email, 
                    c.Address, c.Image, c.Gender
                FROM 
                    registration r
                INNER JOIN 
                    customers c ON r.Id = c.Id
                WHERE 
                    r.Id = @Id";
            var client = await connection.QuerySingleOrDefaultAsync<ClientModel>(sql, new { Id = id });
            return client;
        }



        /// <summary>
        /// //Chart-graph for table
        /// </summary>
        /// <returns></returns>
        #region Order Services

        // Fetch all orders
        //public async Task<IEnumerable<Cart>> GetAllOrdersAsync()
        //{
        //    using var connection = CreateConnection();

        //    // Query to fetch all cart orders
        //    string sql = "SELECT * FROM cart";

        //    // Fetch data from the database
        //    var orders = await connection.QueryAsync<Cart>(sql);

        //    return orders; // Returns the list of cart orders
        //}


        public async Task<IEnumerable<CartItem>> GetAllOrdersAsync()
        {
            using var connection = CreateConnection();

            // SQL query to fetch cart orders with joined data from registration and products tables
            string sql = @"
        SELECT 
    c.date, 
    c.quantity, 
    c.customer_id,
    r.firstName, 
    r.lastName,
    p.product_name, 
    p.product_id,
    p.image_url, 
    p.price
FROM 
    orders c
JOIN 
    registration r ON c.customer_id = r.Id
JOIN 
    products p ON c.product_id = p.product_id";

            // Fetch the data from the database
            var orders = await connection.QueryAsync<CartItem>(sql);

            return orders; // Returns the list of cart orders with joined data
        }





        // Get order by ID
        //public async Task<IEnumerable<Cart>> GetOrderByIdAsync(int customer_id)
        //{
        //    using var connection = CreateConnection();

        //    // Query to fetch all orders for the customer
        //    string sql = "SELECT product_id, date, quantity FROM cart WHERE customer_id = @customer_id;";

        //    // Return a list of carts
        //    return await connection.QueryAsync<Cart>(sql, new { customer_id });
        //}
        public async Task<IEnumerable<DateSummary>> GetOrderByIdAsync(int customer_id)
        {
            using var connection = CreateConnection();

            var result = await connection.QueryAsync<DateSummary>(
                "GetOrderSummaryByDate",
                new { CustomerId = customer_id },
                commandType: CommandType.StoredProcedure
            );

            return result;
        }




        // Create or update an order in the cart
        public async Task<int> CreateOrderAsync(string customerId, string productName)
        {
            using var connection = CreateConnection();

            // Step 1: Fetch product_id by matching productname
            const string getProductSql = "SELECT product_id FROM products WHERE product_name = @ProductName";
            var productId = await connection.QuerySingleOrDefaultAsync<int>(getProductSql, new { ProductName = productName });

            if (productId == 0)
            {
                throw new Exception($"Product with name '{productName}' not found.");
            }

            // Step 2: Check if the product already exists in the cart for the given customer
            const string checkCartSql = "SELECT * FROM orders WHERE customer_id = @CustomerId AND product_id = @ProductId";
            var existingCart = await connection.QuerySingleOrDefaultAsync<Cart>(checkCartSql, new { CustomerId = customerId, ProductId = productId });

            if (existingCart != null)
            {
                // Step 3: If the product exists in the cart, update the quantity
                const string updateCartSql = "UPDATE orders SET quantity = quantity + 1 WHERE customer_id = @CustomerId AND product_id = @ProductId";
                return await connection.ExecuteAsync(updateCartSql, new { CustomerId = customerId, ProductId = productId });
            }
            else
            {
                // Step 4: If the product does not exist in the cart, insert it
                const string insertCartSql = "INSERT INTO orders (customer_id, product_id, date, quantity) VALUES (@CustomerId, @ProductId, @Date, @Quantity)";
                return await connection.ExecuteAsync(insertCartSql, new
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Date = DateTime.Now,
                    Quantity = 1
                });
            }
        }



        // update an order by customer ID (This method was added at the end of the class)
        //public async Task<int> UpdateOrderAsync(int product_id)
        //{
        //    using var connection = CreateConnection();
        //    string sql = "DELETE FROM cart WHERE product_id = @product_id";
        //    return await connection.ExecuteAsync(sql, new { product_id });
        //}
        public async Task<int> UpdateOrderAsync(int product_id, int quantity)
        {
            using var connection = CreateConnection();
            string sql = "UPDATE orders SET quantity = @quantity WHERE product_id = @product_id";
            return await connection.ExecuteAsync(sql, new { product_id, quantity });
        }

        // Delete an order by customer ID (This method was added at the end of the class)
        public async Task<int> DeleteOrderAsync(int product_id)
        {
            using var connection = CreateConnection();
            string sql = "DELETE FROM orders WHERE product_id = @product_id";
            return await connection.ExecuteAsync(sql, new { product_id });
        }




        /// <summary>
        /// seller data for seller dashbaord get product name or toatl quantity 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>


        #endregion



        public async Task<IEnumerable<SellerData>> GetAllSellersDataAsync(int sellerId)
        {
            using var connection = CreateConnection();
            const string query = @"
   SELECT 
    c.quantity, 
    c.customer_id,
    s.first_name, 
    s.last_name,
    p.product_name
FROM 
    orders c
JOIN 
    sellers s ON c.product_id = s.product_id  
JOIN 
    products p ON c.product_id = p.product_id 
WHERE
    s.seller_id = @SellerId
    AND s.userId = (SELECT userId FROM sellers WHERE seller_id = @SellerId);";


            return await connection.QueryAsync<SellerData>(query, new { SellerId = sellerId });
        }






        //nisha ka service code edit wala 

        public async Task<EditModel> GetEditDataById(int Id)
        {
            EditModel client = null;

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                var query = @"
      SELECT 
         r.Id,
         r.FirstName, 
         r.LastName, 
         r.Email, 
         c.Address, 
         c.Image, 
         c.Gender,
         r.MobileNumber 
   FROM 
        registration r
      INNER JOIN 
         customers c 
  ON 
      r.Id = c.Id
  WHERE 
      r.Id = @Id;";

                using (var command = new MySqlCommand(query, connection))
                {
                    // Add the @Id parameter to the command
                    command.Parameters.AddWithValue("@Id", Id);


                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            client = new EditModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FirstName = reader["FirstName"]?.ToString(),
                                LastName = reader["LastName"]?.ToString(),
                                Email = reader["Email"]?.ToString(),
                                Address = reader["Address"]?.ToString(),
                                Image = reader["Image"]?.ToString(),
                                Gender = reader["Gender"]?.ToString(),
                                MobileNumber = reader["MobileNumber"]?.ToString()
                            };
                        }
                    }
                }
            }

            return client ;
        }




        public async Task<bool> UpdateClientData(int id, EditModel updatedClient)
        {
            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                var query = @"
            UPDATE registration r
            INNER JOIN customers c ON r.Id = c.Id
            SET 
               
                r.Email = @Email,
                c.Address = @Address,
                c.image = @image
               
            WHERE 
                r.Id = @Id";

                using (var command = new MySqlCommand(query, connection))
                {


                    command.Parameters.AddWithValue("@Email", updatedClient.Email);
                    command.Parameters.AddWithValue("@Address", updatedClient.Address);
                    command.Parameters.AddWithValue("@Image", updatedClient.Image);

                    command.Parameters.AddWithValue("@Id", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }





        ////////////////////Reset-Password//////////////////

        public async Task<int> UpdateResetPassword(int id, ResetPasswordModel resetPassword)
        {
            // Hash the old password for comparison
            string hashedOldPassword = HashPassword(resetPassword.OldPassword);

            // Hash the new password for storing
            resetPassword.NewPassword = HashPassword(resetPassword.NewPassword);

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                // Update only if the old password matches
                string sql = @"UPDATE registration 
               SET Password = @NewPassword 
               WHERE Id = @Id AND Password = @OldPassword";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@NewPassword", resetPassword.NewPassword);
                    command.Parameters.AddWithValue("@OldPassword", hashedOldPassword);
                    command.Parameters.AddWithValue("@Id", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected; // Returns the number of rows affected
                }
            }
        }

        private string HashPassword(string password)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }



        /// <summary>
 /// Seller-Profile
 /// </summary>
 /// <param name="Id"></param>
 /// <returns></returns>

 public async Task<SellerProfileModel> GetSellerDataById(int Id)
 {
     SellerProfileModel seller = null;

     using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
     {
         await connection.OpenAsync();

         var query = @"
             SELECT 
                 r.Id,
                 r.FirstName, 
                 r.LastName, 
                 r.Email, 
                 s.Address, 
                 s.Image, 
                 s.Gender,
                 r.MobileNumber 
             FROM 
                 registration r
             INNER JOIN 
                 sellers s 
             ON 
                 r.Id = s.userId
             WHERE 
                 r.Id = @Id;";

         using (var command = new MySqlCommand(query, connection))
         {
             // Add the @Id parameter to the command
             command.Parameters.AddWithValue("@Id", Id);

             using (var reader = await command.ExecuteReaderAsync())
             {
                 if (await reader.ReadAsync())
                 {
                     seller = new SellerProfileModel
                     {
                         Id = Convert.ToInt32(reader["Id"]),

                         FirstName = reader["FirstName"]?.ToString(),
                         Address = reader["Address"]?.ToString(),
                         Image = reader["Image"]?.ToString(),
                         Gender = reader["Gender"]?.ToString(),
                         Email = reader["Email"]?.ToString(),
                         MobileNumber = reader["MobileNumber"]?.ToString()
                     };
                 }
             }
         }
     }

     return seller;
 }

 public async Task<bool> UpdateSellerData(int id, SellerProfileModel updatedSeller)
 {
     using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
     {
         await connection.OpenAsync();

         var query = @"
             UPDATE registration r
             INNER JOIN sellers s ON r.Id = s.userId
             SET 
                 
                 s.Address = @Address,
                 r.MobileNumber=@MobileNumber,
                 r.Email =@Email,
                 s.image = @image
              
             WHERE 
                 r.Id = @Id";

         using (var command = new MySqlCommand(query, connection))
         {

             command.Parameters.AddWithValue("@Address", updatedSeller.Address);
             command.Parameters.AddWithValue("@Image", updatedSeller.Image);
             command.Parameters.AddWithValue("@Email", updatedSeller.Email);
             command.Parameters.AddWithValue("@MobileNumber", updatedSeller.MobileNumber);


             command.Parameters.AddWithValue("@Id", id);

             var rowsAffected = await command.ExecuteNonQueryAsync();
             return rowsAffected > 0;
         }
     }
 }



        /////////////////////Seller-Password///////////


        public async Task<int> UpdateSellerPassword(int id, SellerPasswordModel sellerPassword)
        {
            // Hash the old password for comparison
            string hashedOldPassword = HashPassword(sellerPassword.OldPassword);

            // Hash the new password for storing
            sellerPassword.NewPassword = HashPassword(sellerPassword.NewPassword);

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();

                // Update only if the old password matches
                string sql = @"UPDATE registration 
                SET Password = @NewPassword 
                WHERE Id = @Id AND Password = @OldPassword";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@NewPassword", sellerPassword.NewPassword);
                    command.Parameters.AddWithValue("@OldPassword", hashedOldPassword);
                    command.Parameters.AddWithValue("@Id", id);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected; // Returns the number of rows affected
                }

            }
        }



    }

}


   
