namespace Backend.Models
{
    public class CategoryDb
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? Url { get; set; }
        public required string Tabs { get; set; }
    }
}
