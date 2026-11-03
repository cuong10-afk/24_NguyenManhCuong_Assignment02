using _24_NguyenManhCuong_Assignment02_BackEnd.Models;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Interfaces
{
    public interface ISystemAccountRepository : IGenericRepository<SystemAccount>
    {
        Task<SystemAccount?> GetByEmailAsync(string email);
        Task<bool> HasNewsArticlesAsync(short accountId);
        IQueryable<SystemAccount> GetAllWithDetails();
    }
}
