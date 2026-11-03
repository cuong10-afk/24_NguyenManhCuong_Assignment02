namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Models
{
    public class TagViewModel
    {
        public int TagID { get; set; }
        public string? TagName { get; set; }
        public string? Note { get; set; }
    }

    public class NewsTagViewModel
    {
        public string NewsArticleID { get; set; } = string.Empty;
        public int TagID { get; set; }
        public TagViewModel? Tag { get; set; }
    }
}
