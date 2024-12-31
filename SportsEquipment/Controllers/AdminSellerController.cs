using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using SportsEquipment.Interfaces;
using SportsEquipment.Model;
using SportsEquipment.Services;


namespace SportsEquipment.Services
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminSellerController : ControllerBase
    {
        private readonly IAdminSellerService _adminSellerService;
        private readonly ICategoryService _categoryservice;
       


        public AdminSellerController(IAdminSellerService adminSellerService, ICategoryService categoryservice)
        {
            _adminSellerService = adminSellerService;
            _categoryservice = categoryservice;
            //_adminService = adminService;
        }

        // GET: api/AdminSeller/allSeller
        [HttpGet("allSeller")]
        public async Task<IActionResult> GetAllSellers()
        {
            var sellers = await _adminSellerService.GetAllSellersAsync();
            return Ok(sellers);
        }

        // POST: api/AdminSeller/seller
        //[HttpPost("seller")]
        [HttpPost("seller")]
        public async Task<IActionResult> AddSeller([FromBody] AdminSeller adminSeller, string product_name)
        {

            await _adminSellerService.AddSellerAsync(adminSeller, product_name);
            return Ok();
        }


        [HttpPut("sellerby/{id}")]
        public async Task<IActionResult> UpdateSeller(int id, [FromBody] AdminSeller updatedSeller)
        {
            var result = await _adminSellerService.UpdateSellerAsync(id, updatedSeller);
            return Ok(result);
        }


        [HttpDelete("{sellerId}")]
        public async Task<IActionResult> DeleteSeller(int sellerId)
        {
            var result = await _adminSellerService.DeleteSellerAsync(sellerId);
            return Ok(result);
        }





        /// <summary>
        /// category  for admin dashbaord
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("allcategory")]
        public async Task<IActionResult> GetAllCategory()
        {
            var products = await _categoryservice.GetAllCategories();
            return Ok(products);
        }


        [HttpPost("category")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            await _categoryservice.AddCategory(category);
            return Ok();
        }

        [HttpPut("updatecategoryby/{id}")]
        public async Task<IActionResult> EditOrder(int id, [FromBody] Category category)
        {

            await _categoryservice.UpdateCategory(id, category);
            return Ok();
        }


        [HttpDelete("deletecategoryby/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            await _categoryservice.DeleteCategory(id);
            return Ok();
        }



        /// <summary>
        /// admin order part hai 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>


        [HttpGet("allOrders")]
        public async Task<IActionResult> GetAllOrder()
        {
            var orders = await _adminSellerService.GetAllOrders();
            return Ok(orders);
        }

    }

}


