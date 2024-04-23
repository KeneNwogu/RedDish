using System.ComponentModel.DataAnnotations;


namespace RedDish.Models
{
    public class Menu
    {
        [Key]
        public int? Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Category { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Image { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
