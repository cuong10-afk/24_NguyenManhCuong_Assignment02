using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Models
{
    [Table("NewsTag")]
    public class NewsTag
    {
        [MaxLength(20)]
        public string NewsArticleID { get; set; } = null!;

        public int TagID { get; set; }

        [JsonIgnore]
        public NewsArticle NewsArticle { get; set; } = null!;

        public Tag Tag { get; set; } = null!;
    }
}
