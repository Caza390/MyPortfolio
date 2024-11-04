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

        [HttpGet("AllTabs")]
        public async Task<ActionResult<IEnumerable<TabsDb>>> GetTabs()
        {
            return await _context.Tabs.ToListAsync();

        }

        [HttpGet("TabId")]
        public async Task<ActionResult<TabsDb>> GetTabs(int id)
        {
            var tabs = await _context.Tabs.FindAsync(id);

            if (tabs == null)
            {
                return NotFound();
            }

            return tabs;
        }

        [HttpGet("TabsUrl")]
        public async Task<IActionResult> GetTabsByUrl(string url)
        {
            var tabs = await _context.Tabs.FirstOrDefaultAsync(c => c.Url == url);
            if (tabs == null)
            {
                return NotFound();
            }
            return Ok(tabs);
        }

        [HttpPost("AddTab")]
        public async Task<ActionResult<TabsDb>> AddTabs(string title, string subtitle, string url)
        {

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(subtitle) || string.IsNullOrEmpty(url))
            {
                return BadRequest("Please fill all boxes");
            }

            var newTabs = new TabsDb
            {
                Title = title,
                Subtitle = subtitle,
                Url = url
            };

            _context.Tabs.Add(newTabs);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTabs), new { id = newTabs.Id }, newTabs);
        }

        [HttpPut("EditTabTitle")]
        public async Task<IActionResult> EditTitle(int id, string title)
        {
            var tabs = await _context.Tabs.FindAsync(id);

            if (tabs == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(title))
            {
                tabs.Title = title;
            }

            await _context.SaveChangesAsync();

            return Ok(tabs);

        }

        [HttpPut("EditTabSubtitle")]
        public async Task<IActionResult> EditSubtitle(int id, string subtitle)
        {
            var tabs = await _context.Tabs.FindAsync(id);

            if (tabs == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(subtitle))
            {
                tabs.Subtitle = subtitle;
            }

            await _context.SaveChangesAsync();

            return Ok(tabs);

        }

        [HttpPut("EditTabUrl")]
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

        [HttpDelete("DeleteTab")]
        public async Task<IActionResult> DeleteTab(int id)
        {
            var tab = await _context.Tabs.FindAsync(id);

            if (tab == null)
            {
                return NotFound();
            }

            _context.Tabs.Remove(tab);

            await _context.SaveChangesAsync();

            return NoContent();

        }
    }
}
