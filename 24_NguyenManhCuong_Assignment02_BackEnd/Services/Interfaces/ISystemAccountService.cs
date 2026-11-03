using _24_NguyenManhCuong_Assignment02_BackEnd.Models;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces
{
    public interface ISystemAccountService
    {
        IQueryable<SystemAccount> GetAll();
        Task<SystemAccount?> GetByIdAsync(short id);
        Task<SystemAccount?> GetByEmailAsync(string email);
        Task<SystemAccount> CreateAsync(SystemAccount account);
        Task<SystemAccount> UpdateAsync(SystemAccount account);
        Task<bool> DeleteAsync(short id);
        Task<bool> HasNewsArticlesAsync(short id);
    }
}
