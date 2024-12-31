using Dapper;
using MySql.Data.MySqlClient;
using SportsEquipment.Interfaces;
using SportsEquipment.Model;

namespace SportsEquipment.Services
{
    public class AdminSellerService : IAdminSellerService
    {
        private readonly string _connectionString;

        public AdminSellerService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<AdminSeller?> GetSellerByIdAsync(int sellerId)
        {
            using var connection = CreateConnection();
            string query = "SELECT * FROM sportsregistration.sellers WHERE SellerId = @SellerId";
            return await connection.QueryFirstOrDefaultAsync<AdminSeller>(query, new { SellerId = sellerId });
        }

        public async Task<IEnumerable<AdminSeller>> GetAllSellersAsync()
        {
            using var connection = CreateConnection();
            string query = "SELECT * FROM sportsregistration.sellers";
            return await connection.QueryAsync<AdminSeller>(query);
        }

       

        public async Task AddSellerAsync(AdminSeller adminSeller, string product_name)
        {
            using var connection = CreateConnection();
            await connection.OpenAsync();

            // Start a transaction to ensure atomicity
            using var transaction = connection.BeginTransaction();

            try
            {
                // Step 1: Retrieve product_id based on product_name
                string getProductSql = "SELECT product_id FROM products WHERE product_name = @product_name";
                var productId = await connection.ExecuteScalarAsync<int?>(getProductSql, new { product_name = product_name }, transaction);

                if (productId == null)
                {
                    throw new Exception("Product not found with the given product_name");
                }

                // Step 2: Insert seller data with retrieved product_id
                string insertSellerSql = @"INSERT INTO sellers (company_name, gst_number, company_address, company_email, company_mobile_number, logo, first_name, last_name, gender, email, mobile_number, address, status, created_at, updated_at, product_id) 
                                   VALUES (@company_name, @gst_number, @company_address, @company_email, @company_mobile_number, @logo, @first_name, @last_name, @gender, @email, @mobile_number, @address, @status, NOW(), NOW(), @product_id)";

                await connection.ExecuteAsync(insertSellerSql,
                    new
                    {
                        adminSeller.company_name,
                        adminSeller.gst_number,
                        adminSeller.company_address,
                        adminSeller.company_email,
                        adminSeller.company_mobile_number,
                        adminSeller.logo,
                        adminSeller.first_name,
                        adminSeller.last_name,
                        adminSeller.gender,
                        adminSeller.email,
                        adminSeller.mobile_number,
                        adminSeller.address,
                        adminSeller.status,
                        product_id = productId
                    },
                    transaction);

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of an error
                transaction.Rollback();
                throw new Exception("Error adding seller: " + ex.Message);
            }
        }




        public async Task<int> UpdateSellerAsync(int sellerId, AdminSeller updatedSeller)
        {
            using var connection = CreateConnection();

            // The SQL query for updating the seller in the database
            string sql = @"
    UPDATE sellers
    SET 
        company_name = @company_name,
        gst_number = @gst_number,
        company_address = @company_address,
        company_email = @company_email,
        company_mobile_number = @company_mobile_number,
        logo = @logo,
        first_name = @first_name,
        last_name = @last_name,
        gender = @gender,
        email = @email,
        mobile_number = @mobile_number,
        address = @address,
        status = @status,
        updated_at = NOW()
    WHERE seller_id = @seller_Id";

            // Execute the query using the provided updatedSeller and sellerId parameters
            var result = await connection.ExecuteAsync(sql, new
            {
                updatedSeller.company_name, // Keeping the structure consistent with AddSellerAsync
                updatedSeller.gst_number,
                updatedSeller.company_address,
                updatedSeller.company_email,
                updatedSeller.company_mobile_number,
                updatedSeller.logo,
                updatedSeller.first_name,
                updatedSeller.last_name,
                updatedSeller.gender,
                updatedSeller.email,
                updatedSeller.mobile_number,
                updatedSeller.address,
                updatedSeller.status,
                seller_Id = sellerId
            });

            return result; 
        }


        public async Task<bool> DeleteSellerAsync(int sellerId)
        {
            using var connection = CreateConnection();
            string sql = "DELETE FROM sellers WHERE SellerId = @SellerId";
            int rowsAffected = await connection.ExecuteAsync(sql, new { SellerId = sellerId });
            return rowsAffected > 0;
        }





        //<------------------admin orders part---------------------------->


        public async Task<IEnumerable<AdminOrder>> GetAllOrders()
        {
            
            using var connection = CreateConnection();
            var query = @"SELECT 
    o.customer_id,
    r.MobileNumber,
    r.FirstName,  
    p.product_name,
    o.quantity,
    o.PaymentStatus
   FROM 
    orders o

  JOIN 
    registration r ON o.customer_id = r.Id  
   JOIN 
    products p ON o.product_id = p.product_id  
LIMIT 0, 1000;
"; 

                var orders = await connection.QueryAsync<AdminOrder>(query);
                return orders.AsList();
            }
        }
    }
