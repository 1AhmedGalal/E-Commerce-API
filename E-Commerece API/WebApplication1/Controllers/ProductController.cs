using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IDataRepository<Product> _repository;

        public ProductController(IDataRepository<Product> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
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
        public IActionResult GetProductById(int id)
        {
            try
            {
                var product = _repository.GetById(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddProduct([FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _repository.Add(product);
                return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateProductById(int id, [FromBody] Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState); 
                }

                var existingProduct = _repository.GetById(id);
                if (existingProduct == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                _repository.UpdateById(id, product);
                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProductById(int id)
        {
            try
            {
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
