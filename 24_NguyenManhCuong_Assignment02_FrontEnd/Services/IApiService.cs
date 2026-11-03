using _24_NguyenManhCuong_Assignment02_FrontEnd.Models;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Services
{
    public interface IApiService
    {
        // Auth
        Task<(bool Success, LoginResultDto? Data, string? ErrorMessage)> LoginAsync(LoginViewModel model);

        // Accounts (Admin)
        Task<List<SystemAccountViewModel>> GetAccountsAsync(string? search = null);
        Task<SystemAccountViewModel?> GetAccountByIdAsync(short id);
        Task<(bool Success, string? ErrorMessage)> CreateAccountAsync(SystemAccountViewModel model);
        Task<(bool Success, string? ErrorMessage)> UpdateAccountAsync(short id, SystemAccountViewModel model);
        Task<(bool Success, string? ErrorMessage)> DeleteAccountAsync(short id);

        // Profile (Staff)
        Task<ProfileViewModel?> GetProfileAsync(short id);
        Task<(bool Success, string? ErrorMessage)> UpdateProfileAsync(ProfileViewModel model);

        // Categories (Staff)
        Task<List<CategoryViewModel>> GetCategoriesAsync();
        Task<CategoryViewModel?> GetCategoryByIdAsync(short id);
        Task<(bool Success, string? ErrorMessage)> CreateCategoryAsync(CategoryViewModel model);
        Task<(bool Success, string? ErrorMessage)> UpdateCategoryAsync(short id, CategoryViewModel model);
        Task<(bool Success, string? ErrorMessage)> DeleteCategoryAsync(short id);

        // News Articles
        Task<List<NewsArticleViewModel>> GetActiveNewsArticlesAsync(string? search = null, short? categoryId = null);
        Task<List<NewsArticleViewModel>> GetAllNewsArticlesAsync(string? search = null, short? categoryId = null);
        Task<NewsArticleViewModel?> GetNewsArticleByIdAsync(string id);
        Task<List<NewsArticleViewModel>> GetArticlesByCreatorAsync(short accountId);
        Task<(bool Success, string? ErrorMessage)> CreateNewsArticleAsync(NewsArticleViewModel model);
        Task<(bool Success, string? ErrorMessage)> UpdateNewsArticleAsync(string id, NewsArticleViewModel model);
        Task<(bool Success, string? ErrorMessage)> DeleteNewsArticleAsync(string id);

        // Tags
        Task<List<TagViewModel>> GetTagsAsync();

        // Admin Reports
        Task<List<NewsArticleViewModel>> GetReportAsync(DateTime startDate, DateTime endDate);
    }
}
