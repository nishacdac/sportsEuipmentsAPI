using SportsEquipment.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsEquipment.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int id);
        Task AddCategory(Category category);
        Task UpdateCategory(int id, Category category);  
        Task DeleteCategory(int id);
    }
}
