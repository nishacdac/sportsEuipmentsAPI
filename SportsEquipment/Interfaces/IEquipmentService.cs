using System.Threading.Tasks;
using SportsEquipment.Model;

namespace SportsEquipment.Interfaces
{
    public interface IEquipmentService
    {
        Task<bool> SellerEquipment(Equipment equipment);
        Task<string?> LoginUser(EquipmentLogin equipmentLogin);

        Task<string?> RegisterEquipment(AdminLogin adminLogin);
        Task<bool> RegisterCustomerEquipment(Equipment equipment);
    }
}
