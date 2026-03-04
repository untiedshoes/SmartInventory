using Microsoft.AspNetCore.Mvc;
using SmartInventory.Core.Entities;
using SmartInventory.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace SmartInventory.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /api/products
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        // GET: /api/products/top
        [HttpGet("top")]
        public async Task<IActionResult> GetTop([FromQuery] int count = 5)
        {
            var topProducts = await _productService.GetTopAsync(count);
            return Ok(topProducts);
        }

        // New paged endpoint
        // GET: /api/products/paged?page=1&pageSize=20&categoryId=...&search=...
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] string? search = null)
        {
            var (products, totalCount) = await _productService.GetPagedAsync(page, pageSize, categoryId, search);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var response = new
            {
                data = products,
                totalCount,
                page,
                pageSize,
                totalPages
            };

            return Ok(response);
        }

        // GET: /api/products/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // POST: /api/products
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var created = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        // PUT: /api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, Product product)
        {
            if (id != product.Id) return BadRequest();

            await _productService.UpdateAsync(product);
            return NoContent();
        }

        // DELETE: /api/products/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return NoContent();
        }
    }
}