using Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/category")]
    public class CategoryController(ApplicationDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            var cats = await db.Categories
                .OrderBy(c => c.ID)
                .Select(c => new { id = c.ID, name = c.Name })
                .ToListAsync();
            return Ok(cats);
        }
    }
}
