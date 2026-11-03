using _24_NguyenManhCuong_Assignment02_FrontEnd.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace _24_NguyenManhCuong_Assignment02_FrontEnd.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5199";
            _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private void AddAuthHeader()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var token = user?.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<(bool Success, LoginResultDto? Data, string? ErrorMessage)> LoginAsync(LoginViewModel model)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/Auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<LoginResultDto>(responseContent, _jsonOptions);
                    return (true, result, null);
                }

                try
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    if (doc.RootElement.TryGetProperty("message", out var msgElement))
                        return (false, null, msgElement.GetString());
                }
                catch { }

                return (false, null, "Invalid email or password.");
            }
            catch (Exception ex)
            {
                return (false, null, $"Connection error: {ex.Message}");
            }
        }

        // ==================== SYSTEM ACCOUNTS (ADMIN) ====================
        public async Task<List<SystemAccountViewModel>> GetAccountsAsync(string? search = null)
        {
            AddAuthHeader();
            var url = "odata/SystemAccounts?$orderby=AccountID";
            if (!string.IsNullOrWhiteSpace(search))
            {
                url += $"&$filter=contains(tolower(AccountName),'{search.ToLower()}') or contains(tolower(AccountEmail),'{search.ToLower()}')";
            }

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<SystemAccountViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            var odata = JsonSerializer.Deserialize<ODataResponse<SystemAccountViewModel>>(json, _jsonOptions);
            return odata?.Value ?? new List<SystemAccountViewModel>();
        }

        public async Task<SystemAccountViewModel?> GetAccountByIdAsync(short id)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"odata/SystemAccounts/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<SystemAccountViewModel>(json, _jsonOptions);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAccountAsync(SystemAccountViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                model.AccountName,
                model.AccountEmail,
                model.AccountRole,
                model.AccountPassword
            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("odata/SystemAccounts", content);
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAccountAsync(short id, SystemAccountViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                model.AccountID,
                model.AccountName,
                model.AccountEmail,
                model.AccountRole,
                model.AccountPassword
            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"odata/SystemAccounts/{id}", content);
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAccountAsync(short id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"odata/SystemAccounts/{id}");
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        // ==================== PROFILE (STAFF) ====================
        public async Task<ProfileViewModel?> GetProfileAsync(short id)
        {
            AddAuthHeader();
            var account = await GetAccountByIdAsync(id);
            if (account == null) return null;

            return new ProfileViewModel
            {
                AccountID = account.AccountID,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                RoleName = account.RoleDescription
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateProfileAsync(ProfileViewModel model)
        {
            AddAuthHeader();
            var existing = await GetAccountByIdAsync(model.AccountID);
            if (existing == null) return (false, "Account not found.");

            var passwordToSave = !string.IsNullOrWhiteSpace(model.NewPassword) ? model.NewPassword : existing.AccountPassword;

            var payload = new
            {
                model.AccountID,
                model.AccountName,
                model.AccountEmail,
                existing.AccountRole,
                AccountPassword = passwordToSave
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"odata/SystemAccounts/{model.AccountID}", content);
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        // ==================== CATEGORIES (STAFF) ====================
        public async Task<List<CategoryViewModel>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("odata/Categories?$expand=ParentCategory&$orderby=CategoryID");
            if (!response.IsSuccessStatusCode) return new List<CategoryViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            var odata = JsonSerializer.Deserialize<ODataResponse<CategoryViewModel>>(json, _jsonOptions);
            return odata?.Value ?? new List<CategoryViewModel>();
        }

        public async Task<CategoryViewModel?> GetCategoryByIdAsync(short id)
        {
            var response = await _httpClient.GetAsync($"odata/Categories/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoryViewModel>(json, _jsonOptions);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateCategoryAsync(CategoryViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                model.CategoryName,
                model.CategoryDesciption,
                model.ParentCategoryID,
                model.IsActive
            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("odata/Categories", content);
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateCategoryAsync(short id, CategoryViewModel model)
        {
            AddAuthHeader();
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                model.CategoryID,
                model.CategoryName,
                model.CategoryDesciption,
                model.ParentCategoryID,
                model.IsActive
            }), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"odata/Categories/{id}", content);
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteCategoryAsync(short id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"odata/Categories/{id}");
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        // ==================== NEWS ARTICLES ====================
        public async Task<List<NewsArticleViewModel>> GetActiveNewsArticlesAsync(string? search = null, short? categoryId = null)
        {
            var query = "odata/NewsArticles?$filter=NewsStatus eq true&$expand=Category,CreatedBy,NewsTags($expand=Tag)&$orderby=CreatedDate desc";
            if (!string.IsNullOrWhiteSpace(search))
            {
                query += $" and (contains(tolower(Headline),'{search.ToLower()}') or contains(tolower(NewsTitle),'{search.ToLower()}'))";
            }
            if (categoryId.HasValue)
            {
                query += $" and CategoryID eq {categoryId.Value}";
            }

            var response = await _httpClient.GetAsync(query);
            if (!response.IsSuccessStatusCode) return new List<NewsArticleViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            var odata = JsonSerializer.Deserialize<ODataResponse<NewsArticleViewModel>>(json, _jsonOptions);
            return odata?.Value ?? new List<NewsArticleViewModel>();
        }

        public async Task<List<NewsArticleViewModel>> GetAllNewsArticlesAsync(string? search = null, short? categoryId = null)
        {
            AddAuthHeader();
            var query = "odata/NewsArticles?$expand=Category,CreatedBy,NewsTags($expand=Tag)&$orderby=CreatedDate desc";
            var filters = new List<string>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                filters.Add($"(contains(tolower(Headline),'{search.ToLower()}') or contains(tolower(NewsTitle),'{search.ToLower()}'))");
            }
            if (categoryId.HasValue)
            {
                filters.Add($"CategoryID eq {categoryId.Value}");
            }

            if (filters.Any())
            {
                query += "&$filter=" + string.Join(" and ", filters);
            }

            var response = await _httpClient.GetAsync(query);
            if (!response.IsSuccessStatusCode) return new List<NewsArticleViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            var odata = JsonSerializer.Deserialize<ODataResponse<NewsArticleViewModel>>(json, _jsonOptions);
            return odata?.Value ?? new List<NewsArticleViewModel>();
        }

        public async Task<NewsArticleViewModel?> GetNewsArticleByIdAsync(string id)
        {
            var response = await _httpClient.GetAsync($"odata/NewsArticles/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var article = JsonSerializer.Deserialize<NewsArticleViewModel>(json, _jsonOptions);
            if (article?.NewsTags != null)
            {
                article.SelectedTagIds = article.NewsTags.Select(nt => nt.TagID).ToList();
            }
            return article;
        }

        public async Task<List<NewsArticleViewModel>> GetArticlesByCreatorAsync(short accountId)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"odata/NewsArticles/ByCreator/{accountId}");
            if (!response.IsSuccessStatusCode) return new List<NewsArticleViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<NewsArticleViewModel>>(json, _jsonOptions) ?? new List<NewsArticleViewModel>();
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateNewsArticleAsync(NewsArticleViewModel model)
        {
            AddAuthHeader();
            var payload = new
            {
                model.NewsArticleID,
                model.NewsTitle,
                model.Headline,
                model.NewsContent,
                model.NewsSource,
                model.CategoryID,
                model.NewsStatus,
                model.CreatedByID,
                model.UpdatedByID,
                TagIds = model.SelectedTagIds
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("odata/NewsArticles", content);
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateNewsArticleAsync(string id, NewsArticleViewModel model)
        {
            AddAuthHeader();
            var payload = new
            {
                model.NewsArticleID,
                model.NewsTitle,
                model.Headline,
                model.NewsContent,
                model.NewsSource,
                model.CategoryID,
                model.NewsStatus,
                model.CreatedByID,
                model.UpdatedByID,
                TagIds = model.SelectedTagIds
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"odata/NewsArticles/{id}", content);
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteNewsArticleAsync(string id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"odata/NewsArticles/{id}");
            if (response.IsSuccessStatusCode) return (true, null);

            var error = await ExtractErrorMessage(response);
            return (false, error);
        }

        // ==================== TAGS ====================
        public async Task<List<TagViewModel>> GetTagsAsync()
        {
            var response = await _httpClient.GetAsync("odata/Tags?$orderby=TagName");
            if (!response.IsSuccessStatusCode) return new List<TagViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            var odata = JsonSerializer.Deserialize<ODataResponse<TagViewModel>>(json, _jsonOptions);
            return odata?.Value ?? new List<TagViewModel>();
        }

        // ==================== ADMIN REPORT ====================
        public async Task<List<NewsArticleViewModel>> GetReportAsync(DateTime startDate, DateTime endDate)
        {
            AddAuthHeader();
            var payload = new
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("odata/NewsArticles/Report", content);
            if (!response.IsSuccessStatusCode) return new List<NewsArticleViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<NewsArticleViewModel>>(json, _jsonOptions) ?? new List<NewsArticleViewModel>();
        }

        private async Task<string> ExtractErrorMessage(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                    return msgProp.GetString() ?? "Operation failed.";

                if (doc.RootElement.TryGetProperty("error", out var errProp))
                {
                    if (errProp.ValueKind == JsonValueKind.Object && errProp.TryGetProperty("message", out var innerMsg))
                        return innerMsg.GetString() ?? "Operation failed.";
                    return errProp.GetString() ?? "Operation failed.";
                }
            }
            catch { }

            return $"Request failed with status: {response.StatusCode} ({content})";
        }
    }
}
