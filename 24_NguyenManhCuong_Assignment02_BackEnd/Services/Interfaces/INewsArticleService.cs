using _24_NguyenManhCuong_Assignment02_BackEnd.Models;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces
{
    public interface INewsArticleService
    {
        IQueryable<NewsArticle> GetAll();
        Task<NewsArticle?> GetByIdAsync(string id);
        Task<NewsArticle> CreateAsync(NewsArticle article, List<int> tagIds);
        Task<NewsArticle> UpdateAsync(NewsArticle article, List<int> tagIds);
        Task<bool> DeleteAsync(string id);
        Task<IEnumerable<NewsArticle>> GetByCreatedByIdAsync(short accountId);
        Task<IEnumerable<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate);
    }
}
