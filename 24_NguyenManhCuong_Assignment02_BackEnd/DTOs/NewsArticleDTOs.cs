using System.ComponentModel.DataAnnotations;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.DTOs
{
    public class NewsArticleRequest
    {
        [Required(ErrorMessage = "NewsArticleID is required")]
        [MaxLength(20)]
        public string NewsArticleID { get; set; } = null!;

        [MaxLength(400)]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        [MaxLength(150)]
        public string Headline { get; set; } = null!;

        [MaxLength(4000)]
        public string? NewsContent { get; set; }

        [MaxLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryID { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedByID { get; set; }

        public short? UpdatedByID { get; set; }

        public List<int> TagIds { get; set; } = new();
    }

    public class ReportRequest
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
