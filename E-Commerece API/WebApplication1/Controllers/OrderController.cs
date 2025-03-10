using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;
using System.Security.Claims;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IDataRepository<Order> _repository;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(IDataRepository<Order> repository, UserManager<AppUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            try
            {
                return Ok(_repository.GetAll());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var order = _repository.GetById(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult AddOrder([FromBody] int productId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get the logged-in user's ID
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not found. Please log in.");
                }

                // Create the order object
                var order = new Order
                {
                    UserId = userId,
                    ProductID = productId
                };

                _repository.Add(order);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User")]
        public IActionResult UpdateOrderById(int id, [FromBody] int productId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingOrder = _repository.GetById(id);
                if (existingOrder == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (existingOrder.UserId != userId)
                {
                    return Forbid("You are not authorized to modify this order.");
                }

                existingOrder.ProductID = productId;
                _repository.UpdateById(id, existingOrder);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "User")]
        public IActionResult DeleteOrderById(int id)
        {
            try
            {
                var order = _repository.GetById(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                // Ensure the logged-in user is the owner of the order
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (order.UserId != userId)
                {
                    return Forbid("You are not authorized to delete this order.");
                }

                _repository.DeleteById(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
