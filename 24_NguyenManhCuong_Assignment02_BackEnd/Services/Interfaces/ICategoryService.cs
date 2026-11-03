using _24_NguyenManhCuong_Assignment02_BackEnd.Models;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces
{
    public interface ICategoryService
    {
        IQueryable<Category> GetAll();
        Task<Category?> GetByIdAsync(short id);
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(short id);
    }
}
