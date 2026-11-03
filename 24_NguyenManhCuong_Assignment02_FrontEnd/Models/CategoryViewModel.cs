using System.ComponentModel.DataAnnotations;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Models
{
    public class CategoryViewModel
    {
        [Display(Name = "Category ID")]
        public short CategoryID { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [MaxLength(100)]
        [Display(Name = "Category Name")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(250)]
        [Display(Name = "Description")]
        public string CategoryDesciption { get; set; } = string.Empty;

        [Display(Name = "Parent Category")]
        public short? ParentCategoryID { get; set; }

        public string? ParentCategoryName { get; set; }

        public CategoryViewModel? ParentCategory { get; set; }

        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;
    }
}
