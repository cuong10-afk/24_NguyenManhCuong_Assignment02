using _24_NguyenManhCuong_Assignment02_BackEnd.Data;
using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Implementations
{
    public class SystemAccountRepository : GenericRepository<SystemAccount>, ISystemAccountRepository
    {
        public SystemAccountRepository(FUNewsManagementContext context) : base(context) { }

        public async Task<SystemAccount?> GetByEmailAsync(string email)
            => await _dbSet.FirstOrDefaultAsync(a => a.AccountEmail == email);

        public async Task<bool> HasNewsArticlesAsync(short accountId)
            => await _context.NewsArticles.AnyAsync(n => n.CreatedByID == accountId);

        public IQueryable<SystemAccount> GetAllWithDetails()
            => _dbSet.AsQueryable();
    }
}
