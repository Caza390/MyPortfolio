using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubcategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubcategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllSubcategories")]
        public async Task<ActionResult<IEnumerable<SubcategoryDb>>> GetSubcategories()
        {
            return await _context.Subcategory.ToListAsync();
        }

        [HttpGet("subcategoryId")]
        public async Task<ActionResult<SubcategoryDb>> GetSubcategory(int id)
        {
            var subcategory = await _context.Subcategory
                .FirstOrDefaultAsync(c => c.Id == id);

            if (subcategory == null)
            {
                return NotFound();
            }

            return subcategory;
        }

        [HttpGet("SubcategoriesByCategory")]
        public async Task<ActionResult<IEnumerable<SubcategoryDb>>> GetSubcategoriesByCategory(string category)
        {
            var subcategory = await _context.Subcategory
                .Where(c => c.Category == category)
                .ToListAsync();

            if (!subcategory.Any())
            {
                return NotFound();
            }

            return Ok(subcategory);
        }

        [HttpPost("AddSubcategory")]
        public async Task<ActionResult<SubcategoryDb>> AddsubCategory(
            string heading,
            string title,
            string description,
            string? startDate,
            string? endDate,
            string category,
            IFormFile? imageFile)
        {
            if (string.IsNullOrEmpty(heading) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(category))
            {
                return BadRequest("Please fill all required fields.");
            }

            DateOnly? parsedStartDate = null;
            if (!string.IsNullOrEmpty(startDate))
            {
                if (DateOnly.TryParseExact(startDate, "yyyy-MM-dd", out DateOnly tempStartDate))
                {
                    parsedStartDate= tempStartDate;
                }
                else
                {
                    return BadRequest("Invalid end date format. Use yyyy-MM-dd.");
                }
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

            if(parsedEndDate.HasValue && (parsedStartDate == null || parsedEndDate < parsedStartDate))
            {
                return BadRequest("End date cannot be before start date.");
            }

            string? imagePath = null;
            if (imageFile != null)
            {
                var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "StoredImages");
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

                imagePath = $"/StoredImages/{uniqueFileName}";
            }

            var newSubcategory = new SubcategoryDb
            {
                Heading = heading,
                Title = title,
                Description = description,
                StartDate = parsedStartDate,
                EndDate = parsedEndDate,
                Category = category,
                ImagePath = imagePath
            };

            _context.Subcategory.Add(newSubcategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubcategory), new { id = newSubcategory.Id }, newSubcategory);
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(int subcategoryId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("No image file provided.");
            }

            var subcategory = await _context.Subcategory.FindAsync(subcategoryId);
            if (subcategory == null)
            {
                return NotFound("Subcategory not found.");
            }

            var imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "StoredImages", "Subcategory");
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

            subcategory.ImagePath = $"/StoredImages/Subcategory/{uniqueFileName}";
            await _context.SaveChangesAsync();

            return Ok(new { ImagePath = subcategory.ImagePath });
        }


        [HttpPut("EditSubcategoryHeading")]
        public async Task<IActionResult> EditHeading(int id, string heading)
        {
            var subcategory = await _context.Subcategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(heading))
            {
                subcategory.Heading = heading;
            }

            await _context.SaveChangesAsync();

            return Ok(subcategory);
        }

        [HttpPut("EditSubcategoryTitle")]
        public async Task<IActionResult> EditTitle(int id, string title)
        {
            var subcategory = await _context.Subcategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(title))
            {
                subcategory.Title = title;
            }

            await _context.SaveChangesAsync();

            return Ok(subcategory);
        }

        [HttpPut("EditSubcategoryDescription")]
        public async Task<IActionResult> EditDescription(int id, string description)
        {
            var subcategory = await _context.Subcategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(description))
            {
                subcategory.Description = description;
            }

            await _context.SaveChangesAsync();

            return Ok(subcategory);
        }

        [HttpPut("EditSubcategoryStartDate")]
        public async Task<IActionResult> EditStartDate(int id, string? startDateString)
        {
            var subcategory = await _context.Subcategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(startDateString))
            {
                subcategory.StartDate = null;
            }
            else
            {
                if (DateOnly.TryParse(startDateString, out var startDate))
                {
                    subcategory.StartDate = startDate;
                }
                else
                {
                    return BadRequest("Invalid end date format. Use yyyy-MM-dd.");
                }
            }

            await _context.SaveChangesAsync();

            return Ok(subcategory);
        }

        [HttpPut("EditSubcategoryEndDate")]
        public async Task<IActionResult> EditEndDate(int id, string? endDateString)
        {
            var subcategory = await _context.Subcategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(endDateString))
            {
                subcategory.EndDate = null;
            }
            else
            {
                if (DateOnly.TryParse(endDateString, out var endDate))
                {
                    subcategory.EndDate = endDate;
                }
                else
                {
                    return BadRequest("Invalid end date format. Use yyyy-MM-dd.");
                }
            }

            await _context.SaveChangesAsync();

            return Ok(subcategory);
        }


        [HttpPut("EditSubategoryCategory")]
        public async Task<IActionResult> EditCategory(int id, string category)
        {
            var subcategory = await _context.Subcategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(category))
            {
                subcategory.Category = category;
            }

            await _context.SaveChangesAsync();

            return Ok(subcategory);
        }

        [HttpDelete("DeleteSubcategory")]
        public async Task<IActionResult> DeleteSubcategory(int id)
        {
            var subcategory = await _context.Subcategory.FindAsync(id);

            if (subcategory == null)
            {
                return NotFound();
            }

            _context.Subcategory.Remove(subcategory);

            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}
