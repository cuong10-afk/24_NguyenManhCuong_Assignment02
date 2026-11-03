using System.ComponentModel.DataAnnotations;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Models
{
    public class NewsArticleViewModel
    {
        [Required(ErrorMessage = "Article ID is required")]
        [MaxLength(20)]
        [Display(Name = "Article ID")]
        public string NewsArticleID { get; set; } = string.Empty;

        [MaxLength(400)]
        [Display(Name = "Title")]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        [MaxLength(150)]
        [Display(Name = "Headline")]
        public string Headline { get; set; } = string.Empty;

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        [MaxLength(4000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Content")]
        public string? NewsContent { get; set; }

        [MaxLength(400)]
        [Display(Name = "Source")]
        public string? NewsSource { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Display(Name = "Category")]
        public short? CategoryID { get; set; }

        [Display(Name = "Status (Active/Published)")]
        public bool NewsStatus { get; set; } = true;

        public short? CreatedByID { get; set; }
        public short? UpdatedByID { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime? ModifiedDate { get; set; }

        public CategoryViewModel? Category { get; set; }
        public SystemAccountViewModel? CreatedBy { get; set; }
        public List<NewsTagViewModel> NewsTags { get; set; } = new List<NewsTagViewModel>();

        [Display(Name = "Tags")]
        public List<int> SelectedTagIds { get; set; } = new List<int>();
    }
}
