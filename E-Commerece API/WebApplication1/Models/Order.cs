using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public int ProductID { get; set; }

    }
}
