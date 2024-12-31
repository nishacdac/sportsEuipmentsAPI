using Dapper;
using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using SportsEquipment.Interfaces;
using SportsEquipment.Model;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SportsEquipment.Services
{
    /// <summary>
    /// sellerservice is a product for seller pge 
    /// </summary>
    public class SellerService : ISellerService
    //IOrderService
    {
        private readonly string _connectionString;

        public SellerService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // For ISellerService: Get All Products
        public async Task<IEnumerable<products>> GetAllProductsAsync()
        {
            using var connection = CreateConnection();
            string sql = "SELECT * FROM products";
            return await connection.QueryAsync<products>(sql);
        }

        public async Task<int> AddProductAsync(products product, string category_name)
        {
            var sql = @" INSERT INTO products (product_name, material_type, image_url, in_Active, description,price, create_At, update_At, category_id)
 VALUES (@product_name, @material_type, @image_url, @in_Active, @description, @price, @create_At, @update_At,
         (SELECT CategoryId FROM category WHERE Name = @Category_Name));";

            using (var connection = CreateConnection())
            {
                return await connection.ExecuteAsync(sql, new
                {
                    product.product_name,
                    product.material_type,
                    product.image_url,
                    product.in_Active,
                    product.description,
                    product.price,
                    product.create_At,
                    product.update_At,
                    Category_Name = category_name
                });
            }
        }


        public async Task<int> UpdateProductAsync(int id, products updatedProduct)
        {
            using (var connection = CreateConnection())
            {
                var sql = @"UPDATE products 
            SET product_name = @product_name, 
                material_type  = @material_type , 
                image_url = @image_url, 
                in_Active = @in_Active, 
                description = @description,
                price = @price,
                create_At = @create_At,
                update_At = @update_At,
                category_id = @category_id
            WHERE product_ID = @product_ID";

                updatedProduct.product_ID = id;
                return await connection.ExecuteAsync(sql, updatedProduct);
            }
        }


        ///////////////for productChart in Admin/////////

       
        public async Task<List<ProductChart>> GetProductChartDataAsync()
        {

            using var connection = CreateConnection();
            var query = @"SELECT p.product_name, o.product_id, SUM(o.quantity) AS total_quantity
                              FROM products p
                              JOIN orders o ON p.product_id = o.product_id
                              GROUP BY p.product_name, o.product_id";


            var orders = await connection.QueryAsync<ProductChart>(query);
            return orders.AsList();
        }



    }



}
