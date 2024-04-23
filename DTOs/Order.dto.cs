using System.ComponentModel.DataAnnotations;

namespace RedDish.DTOs
{
    public class OrderItemDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 0;

        [Required]
        [Range(1, int.MaxValue)]
        public int MenuId { get; set; } = 0;
    }
}
