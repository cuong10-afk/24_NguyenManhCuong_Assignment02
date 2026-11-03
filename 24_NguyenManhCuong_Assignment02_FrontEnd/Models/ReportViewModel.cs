using System.ComponentModel.DataAnnotations;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Models
{
    public class ReportViewModel
    {
        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "From Date")]
        public DateTime StartDate { get; set; } = DateTime.Today.AddDays(-30);

        [Required(ErrorMessage = "End Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        public DateTime EndDate { get; set; } = DateTime.Today;

        public List<NewsArticleViewModel> Articles { get; set; } = new List<NewsArticleViewModel>();

        public int TotalArticles => Articles.Count;
        public int ActiveArticles => Articles.Count(a => a.NewsStatus);
        public int InactiveArticles => Articles.Count(a => !a.NewsStatus);
    }
}
