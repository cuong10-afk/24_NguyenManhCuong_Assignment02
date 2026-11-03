using System.ComponentModel.DataAnnotations;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Models
{
    public class SystemAccountViewModel
    {
        [Display(Name = "Account ID")]
        public short AccountID { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string AccountName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(70)]
        [Display(Name = "Email Address")]
        public string AccountEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public int? AccountRole { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(70)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string AccountPassword { get; set; } = string.Empty;

        public string RoleDescription => AccountRole switch
        {
            1 => "Staff",
            2 => "Lecturer",
            _ => "Unknown"
        };
    }
}
