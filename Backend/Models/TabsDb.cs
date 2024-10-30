using Microsoft.EntityFrameworkCore;

namespace Backend.Models
{
    public class TabsDb
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Subtitle { get; set; }
        public required string Url { get; set; }
    }
}
