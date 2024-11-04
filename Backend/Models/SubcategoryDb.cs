namespace Backend.Models
{
    public class SubcategoryDb
    {
        public int Id { get; set; }
        public required string Heading { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public required string Category { get; set; }
        public string? ImagePath { get; set; }
    }
}
