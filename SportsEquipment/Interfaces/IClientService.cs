using Mysqlx.Crud;
using SportsEquipment.Model;
using SportsEquipment.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsEquipment.Interfaces
{
    public interface IClientService  
    {
        //Task<IEnumerable<products>> GetAllProductsAsync();
        Task<IEnumerable<products>> GetAllProductsAsync(int? pageIndex, int? pageSize, string? searchValue);

        Task<int> AddProductAsync(products product);
        Task<int> DeleteProductAsync(int productId);


        //<----------------------customer servce--------------------->
        Task<ClientModel> GetClientDataById(int id);





        //<--------------order data for customer table------------------>

        //Task<IEnumerable<Cart>> GetAllOrdersAsync(); 
        Task<IEnumerable<CartItem>>GetAllOrdersAsync();
        Task<IEnumerable<DateSummary>> GetOrderByIdAsync(int customer_id);        
        Task<int> CreateOrderAsync(string customerId, string productName);
        //Task<int> DeleteOrderAsync(int customer_id);
        //Task<int> UpdateOrderAsync(int product_id);
        Task<int> UpdateOrderAsync(int product_id, int quantity);
        Task<int> DeleteOrderAsync(int product_id);





        //<---------------------------------seller data for dashbaord for seller --------------------------------->
      
        Task<IEnumerable<SellerData>> GetAllSellersDataAsync(int userId);



        //<------------------------------------------edit nisha ka code--------------->
        Task<EditModel> GetEditDataById(int Id);
        Task<bool> UpdateClientData(int id, EditModel updatedClient);





        //////////////Reset-Password for customer//////////////////////


        Task<int> UpdateResetPassword(int id, ResetPasswordModel resetPassword);


        //<--------------------------seller profile------------------------->
        Task<SellerProfileModel> GetSellerDataById(int Id);
        Task<bool> UpdateSellerData(int id, SellerProfileModel updatedSeller);


        //////////////Seller-Password////////////////

        Task<int> UpdateSellerPassword(int id, SellerPasswordModel sellerPassword);
    }

}

