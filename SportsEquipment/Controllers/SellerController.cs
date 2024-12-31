using Microsoft.AspNetCore.Mvc;
using SportsEquipment.Interfaces;
using SportsEquipment.Model;
using SportsEquipment.Services;
using System.Threading.Tasks;

namespace SportsEquipment.Controllers
{
    [ApiController]
    [Route("api/seller")]
    public class SellerController : ControllerBase
    {
        private readonly ISellerService _sellerService;

        public SellerController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }


        /// <summary>
        /// product data for 
        /// </summary>
        /// <returns></returns>
        [HttpGet("allproduct")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _sellerService.GetAllProductsAsync();
            return Ok(products);
        }


        [HttpPost("product")]
        public async Task<IActionResult> AddProduct([FromBody] products product, string category_name)
        {
            var result = await _sellerService.AddProductAsync(product, category_name);
            return Ok(result);
        }



        [HttpPut("productby/{id}")]
        public async Task<IActionResult> EditProduct(int id, products updatedProduct)
        {
            var result = await _sellerService.UpdateProductAsync(id, updatedProduct);
            return Ok(result);
        }







        ///////////////////Chart show for Admin Products///////////////


        [HttpGet("ProductChart")]
        public async Task<ActionResult<List<ProductChart>>> GetProductData()
        {
            var data = await _sellerService.GetProductChartDataAsync();
            if (data == null || data.Count == 0)
            {
                return NotFound("No data found.");
            }
            return Ok(data);
        }

    }
}



