using _24_NguyenManhCuong_Assignment02_BackEnd.Models;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<bool> HasNewsArticlesAsync(short categoryId);
        IQueryable<Category> GetAllWithDetails();
    }
}
