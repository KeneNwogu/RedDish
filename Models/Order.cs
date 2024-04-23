using System.ComponentModel.DataAnnotations;

namespace RedDish.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public decimal TaxCharges { get; set; }

        [Required]
        public decimal Discount { get; set; }

        [Required]
        public string Status { get; set; }

        [Required] public bool PaymentSuccess { get; set; }

        [Required] public bool CurrentOrder { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string PaymentStatus { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int MenuId { get; set; }

        [Required]
        public int OrderId { get; set; }

        public virtual Menu? MenuItem { get; set; }
    }

}
