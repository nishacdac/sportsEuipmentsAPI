using MySql.Data.MySqlClient;
using Dapper;
using SportsEquipment.Interfaces;
using SportsEquipment.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsEquipment.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly string _connectionString;

        public CategoryService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private MySqlConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            using (var connection = CreateConnection())
            {
                var query = "SELECT * FROM sportsregistration.category";
                return await connection.QueryAsync<Category>(query);
            }
        }

        public async Task<Category> GetCategoryById(int id)
        {
            using var connection = CreateConnection();
            string sql = "SELECT * FROM category WHERE CategoryId = @id";
            return await connection.QueryFirstOrDefaultAsync<Category>(sql, new { id });
        }

        public async Task AddCategory(Category category)
        {
            using var connection = CreateConnection();
            string sql = "INSERT INTO category (CategoryId,Name, Description, CreatedAt,CategoryStatus) VALUES (@CategoryId,@Name, @Description, @CreatedAt, @CategoryStatus)";
            await connection.ExecuteAsync(sql, category);
        }

        public async Task UpdateCategory(int id, Category category)
        {
            using var connection = CreateConnection();
            string sql = "UPDATE category SET Name = @name, Description = @description, CreatedAt = @createdAt, CategoryStatus = @CategoryStatus WHERE CategoryId = @id ";
            await connection.ExecuteAsync(sql, new { category.Name, category.Description, category.CreatedAt, category.CategoryStatus,  id });
        }

        public async Task DeleteCategory(int id)
        {
            using var connection = CreateConnection();
            string sql = "DELETE FROM category WHERE CategoryId = @id";
            await connection.ExecuteAsync(sql, new { id });
        }
    }



    
}
