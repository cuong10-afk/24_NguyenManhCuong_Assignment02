using _24_NguyenManhCuong_Assignment02_BackEnd.Data;
using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(FUNewsManagementContext context) : base(context) { }

        public async Task<bool> HasNewsArticlesAsync(short categoryId)
            => await _context.NewsArticles.AnyAsync(n => n.CategoryID == categoryId);

        public IQueryable<Category> GetAllWithDetails()
            => _dbSet.Include(c => c.ParentCategory).AsQueryable();
    }
}
