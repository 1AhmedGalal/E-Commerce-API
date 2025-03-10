using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModels
{
    public class UserUpdateViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [MaxLength(30)]
        [Display(Name = "New Password")]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MaxLength(30)]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmedPassword { get; set; }
    }
}
