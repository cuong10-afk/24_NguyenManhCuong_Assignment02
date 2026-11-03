using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Models
{
    [Table("NewsArticle")]
    public class NewsArticle
    {
        [Key]
        [MaxLength(20)]
        public string NewsArticleID { get; set; } = null!;

        [MaxLength(400)]
        public string? NewsTitle { get; set; }

        [Required(ErrorMessage = "Headline is required")]
        [MaxLength(150)]
        public string Headline { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        [MaxLength(4000)]
        public string? NewsContent { get; set; }

        [MaxLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryID { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedByID { get; set; }

        public short? UpdatedByID { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("CategoryID")]
        public Category? Category { get; set; }

        [ForeignKey("CreatedByID")]
        public SystemAccount? CreatedBy { get; set; }

        public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}
