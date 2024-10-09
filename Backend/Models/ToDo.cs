using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class ToDo
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }
    }
}
