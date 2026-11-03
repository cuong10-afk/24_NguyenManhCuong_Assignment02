using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Interfaces;
using _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Services.Implementations
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _repository;

        public NewsArticleService(INewsArticleRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<NewsArticle> GetAll() => _repository.GetAllWithDetails();

        public async Task<NewsArticle?> GetByIdAsync(string id)
            => await _repository.GetByIdWithDetailsAsync(id);

        public async Task<NewsArticle> CreateAsync(NewsArticle article, List<int> tagIds)
        {
            article.CreatedDate = DateTime.Now;
            article.ModifiedDate = DateTime.Now;
            await _repository.AddAsync(article);
            await _repository.SaveChangesAsync();

            if (tagIds.Any())
            {
                await _repository.UpdateTagsAsync(article.NewsArticleID, tagIds);
                await _repository.SaveChangesAsync();
            }

            return (await _repository.GetByIdWithDetailsAsync(article.NewsArticleID))!;
        }

        public async Task<NewsArticle> UpdateAsync(NewsArticle article, List<int> tagIds)
        {
            article.ModifiedDate = DateTime.Now;
            _repository.Update(article);
            await _repository.UpdateTagsAsync(article.NewsArticleID, tagIds);
            await _repository.SaveChangesAsync();
            return (await _repository.GetByIdWithDetailsAsync(article.NewsArticleID))!;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var article = await _repository.GetByIdWithDetailsAsync(id);
            if (article == null) return false;
            _repository.Delete(article);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<NewsArticle>> GetByCreatedByIdAsync(short accountId)
            => await _repository.GetByCreatedByIdAsync(accountId);

        public async Task<IEnumerable<NewsArticle>> GetReportAsync(DateTime startDate, DateTime endDate)
            => await _repository.GetByDateRangeAsync(startDate, endDate.Date.AddDays(1).AddSeconds(-1));
    }
}
