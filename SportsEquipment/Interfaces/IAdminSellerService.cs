using Mysqlx.Crud;
using SportsEquipment.Model;

namespace SportsEquipment.Interfaces
{
    public interface IAdminSellerService
    {
        Task<AdminSeller?> GetSellerByIdAsync(int sellerId); 
        Task<IEnumerable<AdminSeller>> GetAllSellersAsync(); 
        Task AddSellerAsync(AdminSeller adminSeller, string product_name);
        Task<bool> DeleteSellerAsync(int sellerId); 

        Task<int> UpdateSellerAsync(int sellerId, AdminSeller updatedSeller);

        //<---------------------------admin order wala part hai --------------------------------------------->

        Task<IEnumerable<AdminOrder>> GetAllOrders();

    }

   
    
}
