using System.Collections.Generic;
using System.Threading.Tasks;
using SportsEquipment.Model;

namespace SportsEquipment.Interfaces
{
    public interface ISellerService
    {

        Task<IEnumerable<products>> GetAllProductsAsync();
        Task<int> AddProductAsync(products product, string category_name);
     
        Task<int> UpdateProductAsync(int id, products updatedProduct);


        ////////////for product graph////////////


        
        Task<List<ProductChart>> GetProductChartDataAsync();
    }
    

}



