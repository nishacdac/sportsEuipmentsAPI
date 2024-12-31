 using Microsoft.AspNetCore.Mvc;
using SportsEquipment.Interfaces;
using SportsEquipment.Services;
using SportsEquipment.Model;
using SportsEquipment.Models;
using Mysqlx.Crud;


namespace SportsEquipment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase 
    {
        private readonly IClientService _clientService; 

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;

        }

        // Get all products


        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts(int pageIndex, int pageSize, string searchValue = "")
        {
            // If searchValue is empty, it will be passed as null
            var result = await _clientService.GetAllProductsAsync(pageIndex, pageSize, searchValue);

            if (result == null || !result.Any())
            {
                return NotFound(); 
            }

            return Ok(result); 
        }

        // Add a product
        [HttpPost("product")]
        public async Task<IActionResult> AddProduct([FromBody] products product)
        {
            if (product == null)
            {
                return BadRequest(new { message = "Invalid product data." });
            }

            await _clientService.AddProductAsync(product);
            return Ok(new { message = "Product added successfully." });
        }

        // Delete a product
        [HttpDelete("product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deleted = await _clientService.DeleteProductAsync(id);
            if (deleted == 0)
            {
                return NotFound(new { message = "Product not found." });
            }

            return Ok(new { message = "Product deleted successfully." });
        }


        /// <summary>
        /// customer servcice
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetClientData(int id)
        {
            var equipment = await _clientService.GetClientDataById(id);


            return Ok(equipment);
        }



        /// <summary>
        /// order data for customer pge
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 


        // Get all orders
        [HttpGet("cart")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _clientService.GetAllOrdersAsync();
            if (orders == null || !orders.Any())
            {
                return NotFound(new { message = "No orders found." });
            }

            return Ok(orders);
        }


        // Get order by ID
        [HttpGet("cart/{customer_id}")]
        public async Task<IActionResult> GetOrderById(int customer_id)
        {
            var orders = await _clientService.GetOrderByIdAsync(customer_id);
            if (orders == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            return Ok(orders);
        }


        //// Create a new order
        [HttpPost]
        public async Task<IActionResult> CreateOrder(string customerId, string productName)
        {
           
            var result = await _clientService.CreateOrderAsync(customerId, productName);
            return Ok();
        }



        // update a product
        [HttpPut("Updatecart/{product_id}")]
        public async Task<IActionResult> UpdateOrder(int product_id, int quantity)
        {
            var deleted = await _clientService.UpdateOrderAsync(product_id, quantity);

            return Ok(new { message = "Product update successfully." });
        }

        // Delete a product
        [HttpDelete("cart/{product_id}")]
        public async Task<IActionResult> DeleteOrder(int product_id)
        {
            var deleted = await _clientService.DeleteOrderAsync(product_id);

            return Ok(new { message = "Product deleted successfully." });
        }




        //<-------------------------seller data for seller dashbaord----------------------------------------->
       

        [HttpGet("sellerData/{userId}")]
        public async Task<IActionResult> GetAllSellerData(int userId)
        {
            var sellerData = await _clientService.GetAllSellersDataAsync(userId);
            return sellerData.Any()
                ? Ok(sellerData)
                : NotFound(new { message = "No seller data found." });
        }



        //<-------------------------customer data fro seller dashbaord----------------------------------------->
        


        //nisha da controller code edit wala 



        // [Authorize]
        [HttpGet("get-client-info/{id?}")]
        public async Task<IActionResult> GetEditDataById(int id)
        {


            var equipment = await _clientService.GetEditDataById(id);
            return Ok(equipment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClientData(int id, [FromBody] EditModel updatedClient)
        {
            //var getClientId = User.FindFirst("id").Value;
            //var id = Convert.ToInt32(getClientId);

            if (updatedClient == null || id <= 0)
            {
                return BadRequest("Invalid client data or ID.");
            }

            var result = await _clientService.UpdateClientData(id, updatedClient);

            if (result)
            {
                return Ok(new { message = "Client data updated successfully." });
            }
            else
            {
                return NotFound(new { message = "Client not found or update failed." });
            }
        }






        /// <summary>
        /// ///////////Reset-Password
        /// </summary>
        /// <param name="Reset_passworde"></param>
        /// <returns></returns>
        [HttpPut("reset-password/{id}")]
        public async Task<IActionResult> UpdateResetPassword(int id, [FromBody] ResetPasswordModel resetPassword)
        {
            if (resetPassword == null || id <= 0)
            {
                return BadRequest(new { Message = "Invalid data provided." });
            }

            try
            {
                int result = await _clientService.UpdateResetPassword(id, resetPassword);
                if (result > 0)
                {
                    return Ok(new { Message = "Password updated successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Old password does not match or user not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }






        /// <summary>
        /// /////////////Seller Profile
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        [HttpGet("get-seller-profile")]
        public async Task<IActionResult> GetSellerDataById(int Id)
        {
            // Use the correct service variable name with consistent casing.
            var equipment = await _clientService.GetSellerDataById(Id);
            return Ok(equipment);
        }

        [HttpPut("update-profile-id")]
        public async Task<IActionResult> UpdateSellerProfileData([FromBody] SellerProfileModel updatedSellerProfile)
        {
            if (updatedSellerProfile.Id <= 0)
            {
                return BadRequest("Invalid Seller ID.");
            }

            // Use the correct service variable name with consistent casing.
            var result = await _clientService.UpdateSellerData(updatedSellerProfile.Id, updatedSellerProfile);

            if (result)
            {
                return Ok(new { message = "Seller Profile data updated successfully." });
            }
            else
            {
                return NotFound(new { message = "Seller Profile not found or update failed." });
            }
        }








        ////////////// Seller-Password////////////

        [HttpPut("seller-password/{id}")]
        public async Task<IActionResult> UpdateSellerPassword(int id, [FromBody] SellerPasswordModel sellerPassword)
        {
            if (sellerPassword == null || id <= 0)
            {
                return BadRequest(new { Message = "Invalid data provided." });
            }

            try
            {
                int result = await _clientService.UpdateSellerPassword(id, sellerPassword);
                if (result > 0)
                {
                    return Ok(new { Message = "Password updated successfully." });
                }
                else
                {
                    return BadRequest(new { Message = "Old password does not match or user not found." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Details = ex.Message });
            }
        }

    }
}

