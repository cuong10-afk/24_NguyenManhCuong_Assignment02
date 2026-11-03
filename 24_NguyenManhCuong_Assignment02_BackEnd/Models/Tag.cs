using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Models
{
    [Table("Tag")]
    public class Tag
    {
        [Key]
        public int TagID { get; set; }

        [MaxLength(50)]
        public string? TagName { get; set; }

        [MaxLength(400)]
        public string? Note { get; set; }

        [JsonIgnore]
        public ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}
