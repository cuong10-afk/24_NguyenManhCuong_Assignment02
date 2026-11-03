using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public short CategoryID { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(100)]
        public string CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [MaxLength(250)]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryID { get; set; }

        public bool? IsActive { get; set; }

        [ForeignKey("ParentCategoryID")]
        [JsonIgnore]
        public Category? ParentCategory { get; set; }

        [JsonIgnore]
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

        [JsonIgnore]
        public ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
    }
}
