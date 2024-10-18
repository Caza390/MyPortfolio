namespace Backend.Models
{
    public class TabsDb
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Url { get; set; }
    }
}
