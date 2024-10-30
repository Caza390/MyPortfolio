using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllCategories")]
        public async Task<ActionResult<IEnumerable<CategoryDb>>> GetCategories()
        {
            return await _context.Category.ToListAsync();
        }

        [HttpGet("CategoryId")]
        public async Task<ActionResult<CategoryDb>> GetCategory(int id)
        {
            var category = await _context.Category
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        [HttpGet("CategoriesByTabs")]
        public async Task<ActionResult<IEnumerable<CategoryDb>>> GetCategoriesByTab(string tabs)
        {
            var categories = await _context.Category
                .Where(c => c.Tabs == tabs)
                .ToListAsync();

            if (!categories.Any())
            {
                return NotFound();
            }

            return Ok(categories);
        }

        [HttpPost("AddCategory")]
        public async Task<ActionResult<CategoryDb>> AddCategory(
            string title,
            string description,
            string? url,
            string startDate, // Accepting as string
            string? endDate,  // Nullable string for optional end date
            string tabs)
        {
            // Basic validation to ensure required fields are provided
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(tabs))
            {
                return BadRequest("Please fill all required fields.");
            }

            // Parse startDate
            if (!DateOnly.TryParseExact(startDate, "yyyy-MM-dd", out DateOnly parsedStartDate))
            {
                return BadRequest("Invalid start date format. Use yyyy-MM-dd.");
            }

            // Parse endDate if provided
            DateOnly? parsedEndDate = null;
            if (!string.IsNullOrEmpty(endDate))
            {
                if (DateOnly.TryParseExact(endDate, "yyyy-MM-dd", out DateOnly tempEndDate))
                {
                    parsedEndDate = tempEndDate;
                }
                else
                {
                    return BadRequest("Invalid end date format. Use yyyy-MM-dd.");
                }
            }

            // Additional check: Ensure endDate is after startDate, if endDate is provided
            if (parsedEndDate.HasValue && parsedEndDate < parsedStartDate)
            {
                return BadRequest("End date cannot be before start date.");
            }

            // Create new category entity
            var newCategory = new CategoryDb
            {
                Title = title,
                Description = description,
                Url = url,
                StartDate = parsedStartDate,
                EndDate = parsedEndDate,
                Tabs = tabs
            };

            // Save the new category to the database
            _context.Category.Add(newCategory);
            await _context.SaveChangesAsync();

            // Return the created category details
            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Id }, newCategory);
        }

        [HttpPut("EditCategoryTitle")]
        public async Task<IActionResult> EditName(int id, string title)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(title))
            {
                category.Title = title;
            }

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        [HttpPut("EditCategoryDescription")]
        public async Task<IActionResult> EditDescription(int id, string description)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(description))
            {
                category.Description = description;
            }

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        [HttpPut("EditCategoryUrl")]
        public async Task<IActionResult> EditUrl(int id, string url)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(url))
            {
                category.Url = url;
            }

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        [HttpPut("EditCategoryStartDate")]
        public async Task<IActionResult> EditStartDate(int id, string startDateString)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            // Attempt to parse the date string
            if (DateOnly.TryParse(startDateString, out var startDate))
            {
                if (startDate != DateOnly.MinValue)
                {
                    category.StartDate = startDate;
                }
            }
            else
            {
                // Return a bad request if parsing fails
                return BadRequest("Invalid date format. Please use YYYY-MM-DD.");
            }

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        [HttpPut("EditCategoryEndDate")]
        public async Task<IActionResult> EditEndDate(int id, string? endDateString)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            // Check if endDateString is null or empty
            if (string.IsNullOrEmpty(endDateString))
            {
                // Set the EndDate to null
                category.EndDate = null;
            }
            else
            {
                // Attempt to parse the date string
                if (DateOnly.TryParse(endDateString, out var endDate))
                {
                    category.EndDate = endDate;
                }
                else
                {
                    return BadRequest("Invalid end date format. Use yyyy-MM-dd.");
                }
            }

            await _context.SaveChangesAsync();

            return Ok(category);
        }


        [HttpPut("EditCategoryTabs")]
        public async Task<IActionResult> EditTabs(int id, string tabs)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(tabs))
            {
                category.Tabs = tabs;
            }

            await _context.SaveChangesAsync();

            return Ok(category);
        }
    }
}
