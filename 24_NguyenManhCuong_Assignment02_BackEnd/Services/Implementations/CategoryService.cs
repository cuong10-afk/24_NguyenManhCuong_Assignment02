using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using _24_NguyenManhCuong_Assignment02_BackEnd.Repositories.Interfaces;
using _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<Category> GetAll() => _repository.GetAllWithDetails();

        public async Task<Category?> GetByIdAsync(short id)
            => await _repository.GetByIdAsync(id);

        public async Task<Category> CreateAsync(Category category)
        {
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _repository.Update(category);
            await _repository.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteAsync(short id)
        {
            if (await _repository.HasNewsArticlesAsync(id))
                return false;

            var category = await _repository.GetByIdAsync(id);
            if (category == null) return false;

            _repository.Delete(category);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
