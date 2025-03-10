using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class UserLoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [MaxLength(30)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(30)]
        public string? Password { get; set; }

        public bool RememberMe { get; set; } = false;

    }
}
