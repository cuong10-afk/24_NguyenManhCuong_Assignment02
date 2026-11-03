using _24_NguyenManhCuong_Assignment02_BackEnd.DTOs;
using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Controllers
{
    public class NewsArticlesController : ODataController
    {
        private readonly INewsArticleService _service;

        public NewsArticlesController(INewsArticleService service)
        {
            _service = service;
        }

        [HttpGet("odata/NewsArticles")]
        [EnableQuery(PageSize = 100)]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("odata/NewsArticles/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(string id)
        {
            var article = await _service.GetByIdAsync(id);
            if (article == null) return NotFound(new { message = $"NewsArticle with ID '{id}' not found." });
            return Ok(article);
        }

        [HttpPost("odata/NewsArticles")]
        [Authorize(Roles = "Staff,Lecturer")]
        public async Task<IActionResult> Post([FromBody] NewsArticleRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var article = new NewsArticle
            {
                NewsArticleID = request.NewsArticleID,
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryID = request.CategoryID,
                NewsStatus = request.NewsStatus,
                CreatedByID = request.CreatedByID,
                UpdatedByID = request.UpdatedByID
            };

            var created = await _service.CreateAsync(article, request.TagIds);
            return Created($"odata/NewsArticles('{created.NewsArticleID}')", created);
        }

        [HttpPut("odata/NewsArticles/{id}")]
        [Authorize(Roles = "Staff,Lecturer")]
        public async Task<IActionResult> Put(string id, [FromBody] NewsArticleRequest request)
        {
            if (id != request.NewsArticleID) return BadRequest(new { message = "ID mismatch." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var article = new NewsArticle
            {
                NewsArticleID = request.NewsArticleID,
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryID = request.CategoryID,
                NewsStatus = request.NewsStatus,
                CreatedByID = request.CreatedByID,
                UpdatedByID = request.UpdatedByID
            };

            var updated = await _service.UpdateAsync(article, request.TagIds);
            return Ok(updated);
        }

        [HttpDelete("odata/NewsArticles/{id}")]
        [Authorize(Roles = "Staff,Lecturer")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound(new { message = $"NewsArticle with ID '{id}' not found." });
            return NoContent();
        }

        [HttpGet("odata/NewsArticles/ByCreator/{accountId}")]
        [Authorize]
        public async Task<IActionResult> GetByCreator(short accountId)
        {
            var articles = await _service.GetByCreatedByIdAsync(accountId);
            return Ok(articles);
        }

        [HttpPost("odata/NewsArticles/Report")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetReport([FromBody] ReportRequest request)
        {
            if (request.StartDate > request.EndDate)
                return BadRequest(new { message = "StartDate must be before EndDate." });

            var articles = await _service.GetReportAsync(request.StartDate, request.EndDate);
            return Ok(articles);
        }
    }
}
