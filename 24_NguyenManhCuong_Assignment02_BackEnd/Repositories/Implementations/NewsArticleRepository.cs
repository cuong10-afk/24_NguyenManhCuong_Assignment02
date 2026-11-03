using _24_NguyenManhCuong_Assignment02_BackEnd.Data;
using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Implementations
{
    public class NewsArticleRepository : GenericRepository<NewsArticle>, INewsArticleRepository
    {
        public NewsArticleRepository(FUNewsManagementContext context) : base(context) { }

        public IQueryable<NewsArticle> GetAllWithDetails()
            => _dbSet
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .AsQueryable();

        public async Task<NewsArticle?> GetByIdWithDetailsAsync(string id)
            => await _dbSet
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .FirstOrDefaultAsync(n => n.NewsArticleID == id);

        public async Task<IEnumerable<NewsArticle>> GetByCreatedByIdAsync(short accountId)
            => await _dbSet
                .Include(n => n.Category)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.CreatedByID == accountId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

        public async Task<IEnumerable<NewsArticle>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
            => await _dbSet
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.CreatedDate >= startDate && n.CreatedDate <= endDate)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();

        public async Task UpdateTagsAsync(string newsArticleId, List<int> tagIds)
        {
            var existingTags = await _context.NewsTags
                .Where(nt => nt.NewsArticleID == newsArticleId)
                .ToListAsync();
            _context.NewsTags.RemoveRange(existingTags);

            foreach (var tagId in tagIds)
            {
                await _context.NewsTags.AddAsync(new NewsTag
                {
                    NewsArticleID = newsArticleId,
                    TagID = tagId
                });
            }
        }
    }
}
