using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TabsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TabsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<TabsDb>>> GetTabs()
        {
            return await _context.Tabs.ToListAsync();
        }

        [HttpGet("Id")]
        public async Task<ActionResult<TabsDb>> GetTabs(int id)
        {
            var tabs = await _context.Tabs.FindAsync(id);

            if (tabs == null)
            {
                return NotFound();
            }

            return tabs;
        }

        [HttpGet("Url")]
        public async Task<IActionResult> GetTabsByUrl(string url)
        {
            var tabs = await _context.Tabs.FirstOrDefaultAsync(c => c.Url == url);
            if (tabs == null)
            {
                return NotFound();
            }
            return Ok(tabs);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<TabsDb>> AddTabs(string name, string description, string url)
        {

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(url))
            {
                return BadRequest("Name and Description are required.");
            }

            var newTabs = new TabsDb
            {
                Name = name,
                Description = description,
                Url = url
            };

            _context.Tabs.Add(newTabs);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTabs), new { id = newTabs.Id }, newTabs);
        }

        [HttpPut("EditName")]
        public async Task<IActionResult> EditName(int id, string name)
        {
            var tabs = await _context.Tabs.FindAsync(id);

            if (tabs == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(name))
            {
                tabs.Name = name;
            }

            await _context.SaveChangesAsync();

            return Ok(tabs);

        }

        [HttpPut("EditDescription")]
        public async Task<IActionResult> EditDescription(int id, string description)
        {
            var tabs = await _context.Tabs.FindAsync(id);

            if (tabs == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(description))
            {
                tabs.Description = description;
            }

            await _context.SaveChangesAsync();

            return Ok(tabs);

        }

        [HttpPut("EditUrl")]
        public async Task<IActionResult> EditUrl(int id, string url)
        {
            var tabs = await _context.Tabs.FindAsync(id);

            if (tabs == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(url))
            {
                tabs.Url = url;
            }

            await _context.SaveChangesAsync();

            return Ok(tabs);

        }
    }
}
