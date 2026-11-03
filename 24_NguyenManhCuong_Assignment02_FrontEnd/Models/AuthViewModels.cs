using System.ComponentModel.DataAnnotations;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResultDto
    {
        public string Token { get; set; } = string.Empty;
        public short? AccountID { get; set; }
        public string? AccountName { get; set; }
        public string? AccountEmail { get; set; }
        public int? AccountRole { get; set; }
    }

    public class ProfileViewModel
    {
        public short AccountID { get; set; }

        [Required(ErrorMessage = "Account Name is required")]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Account Email is required")]
        [EmailAddress]
        [MaxLength(70)]
        [Display(Name = "Email Address")]
        public string AccountEmail { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string? RoleName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
        [Display(Name = "New Password (leave blank to keep unchanged)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmPassword { get; set; }
    }
}
