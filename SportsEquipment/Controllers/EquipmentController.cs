using Microsoft.AspNetCore.Mvc;
using SportsEquipment.Interfaces;
using SportsEquipment.Model;
using SportsEquipment.Services;
using System.Threading.Tasks;

namespace SportsEquipment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;

        public EquipmentController(IEquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        // POST api/equipment/register
        [HttpPost("register")]
        public async Task<IActionResult> SellerEquipment([FromBody] Equipment equipment)
        {
            var result = await _equipmentService.SellerEquipment(equipment);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Failed to register equipment");
            }
        }


        // POST api/equipment/register
        [HttpPost("Customerregister")]
        public async Task<IActionResult> RegisterCustomerEquipment([FromBody] Equipment equipment)
        {
            var result = await _equipmentService.RegisterCustomerEquipment(equipment);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Failed to register equipment");
            }
        }

        // POST api/equipment/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] EquipmentLogin equipmentLogin)
        {
            if (equipmentLogin == null)
            {
                return BadRequest("Invalid request body.");
            }

            var token = await _equipmentService.LoginUser(equipmentLogin);
            if (token != null)
            {
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid login credentials");
        }

        [HttpPost("adminloginlogin")]
        public async Task<IActionResult> AdminData([FromBody] AdminLogin adminLogin)
        {
            if (adminLogin == null)
            {
                return BadRequest("Invalid request body.");
            }

            var token = await _equipmentService.RegisterEquipment(adminLogin); // Issue here
            if (token != null)
            {
                return Ok(new { Token = token });
            }
            return Unauthorized("Invalid login credentials");
        }
    }



}



