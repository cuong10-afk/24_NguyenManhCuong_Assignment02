using _24_NguyenManhCuong_Assignment02_BackEnd.Data;
using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Services.Implementations
{
    /// <summary>
    /// TagService is registered as Singleton (Singleton Pattern).
    /// Tags are reference/lookup data that rarely change, making them ideal for singleton caching.
    /// </summary>
    public class TagService : ITagService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TagService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public IQueryable<Tag> GetAll()
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FUNewsManagementContext>();
            return context.Tags.AsNoTracking().ToList().AsQueryable();
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FUNewsManagementContext>();
            return await context.Tags.AsNoTracking().FirstOrDefaultAsync(t => t.TagID == id);
        }
    }
}
