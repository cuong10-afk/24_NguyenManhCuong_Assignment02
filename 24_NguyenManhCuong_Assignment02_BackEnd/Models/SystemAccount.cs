using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Models
{
    [Table("SystemAccount")]
    public class SystemAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short AccountID { get; set; }

        [MaxLength(100)]
        public string? AccountName { get; set; }

        [MaxLength(70)]
        public string? AccountEmail { get; set; }

        public int? AccountRole { get; set; }

        [MaxLength(70)]
        public string? AccountPassword { get; set; }

        [JsonIgnore]
        public ICollection<NewsArticle> CreatedNewsArticles { get; set; } = new List<NewsArticle>();
    }
}
