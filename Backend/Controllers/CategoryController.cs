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

        [HttpGet("CategoryUrl")]
        public async Task<IActionResult> GetCategoryByUrl(string url)
        {
            var category = await _context.Category.FirstOrDefaultAsync(c => c.Url == url);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
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
            string startDate,
            string? endDate,
            string tabs,
            IFormFile? imageFile)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(tabs))
            {
                return BadRequest("Please fill all required fields.");
            }

            if (!DateOnly.TryParseExact(startDate, "yyyy-MM-dd", out DateOnly parsedStartDate))
            {
                return BadRequest("Invalid start date format. Use yyyy-MM-dd.");
            }

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

            if (parsedEndDate.HasValue && parsedEndDate < parsedStartDate)
            {
                return BadRequest("End date cannot be before start date.");
            }

            string? imagePath = null;
            if (imageFile != null)
            {
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "StoredImages", "Category");
                if (!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                var filePath = Path.Combine(imageFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                imagePath = $"/StoredImages/Category/{uniqueFileName}";
            }

            var newCategory = new CategoryDb
            {
                Title = title,
                Description = description,
                Url = url,
                StartDate = parsedStartDate,
                EndDate = parsedEndDate,
                Tabs = tabs,
                ImagePath = imagePath
            };

            _context.Category.Add(newCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = newCategory.Id }, newCategory);
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(int categoryId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("No image file provided.");
            }

            var category = await _context.Category.FindAsync(categoryId);
            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "StoredImages", "Category");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(imageFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            category.ImagePath = $"/StoredImages/Category/{uniqueFileName}";
            await _context.SaveChangesAsync();

            return Ok(new { ImagePath = category.ImagePath });
        }

        [HttpPut("EditCategoryTitle")]
        public async Task<IActionResult> EditTitle(int id, string title)
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

            if (DateOnly.TryParse(startDateString, out var startDate))
            {
                if (startDate != DateOnly.MinValue)
                {
                    category.StartDate = startDate;
                }
            }
            else
            {
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

            if (string.IsNullOrEmpty(endDateString))
            {
                category.EndDate = null;
            }
            else
            {
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

        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Category.Remove(category);

            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}
