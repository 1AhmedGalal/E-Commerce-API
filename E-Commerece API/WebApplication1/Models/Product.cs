using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(20)]
        [MaxLength(50)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }


    }
}
