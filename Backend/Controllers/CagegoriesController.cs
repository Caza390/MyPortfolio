using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpGet("byname/{name}")]
        public async Task<ActionResult<Category>> GetCategoryByName(string name)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }


        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory([FromBody] Category newCategory)
        {

            if (string.IsNullOrEmpty(newCategory.Name) || string.IsNullOrEmpty(newCategory.Description) || string.IsNullOrEmpty(newCategory.Url))
            {
                return BadRequest("Name and Description are required.");
            }

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Id }, newCategory);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditCategory(int id, [FromBody] Category updatedCategory)
        {
            // Find the category by ID
            var existingCategory = await _context.Categories.FindAsync(id);

            if (existingCategory == null)
            {
                return NotFound("Category not found.");
            }

            if (!string.IsNullOrEmpty(updatedCategory.Name))
            {
                existingCategory.Name = updatedCategory.Name;
            }

            if (!string.IsNullOrEmpty(updatedCategory.Description))
            {
                existingCategory.Description = updatedCategory.Description;
            }

            if (!string.IsNullOrEmpty(updatedCategory.Url))
            {
                existingCategory.Url = updatedCategory.Url.ToLower();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(e => e.Id == id))
                {
                    return NotFound("Category not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

    }
}
