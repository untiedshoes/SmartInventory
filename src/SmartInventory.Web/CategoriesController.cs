using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartInventory.Core.Entities;
using SmartInventory.Data;

namespace SmartInventory.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public CategoriesController(InventoryDbContext context)
        {
            _context = context;
        }

        // GET: /api/categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            // AsNoTracking for read-only queries
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Ok(categories);
        }
    }
}