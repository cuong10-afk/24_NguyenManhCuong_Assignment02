using _24_NguyenManhCuong_Assignment02_BackEnd.Models;
using _24_NguyenManhCuong_Assignment02_BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace _24_NguyenManhCuong_Assignment02_BackEnd.Controllers
{
    /// <summary>
    /// Category management OData Controller.
    /// GET is public; CUD requires Staff or Lecturer role.
    /// </summary>
    public class CategoriesController : ODataController
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet("odata/Categories")]
        [EnableQuery(PageSize = 100)]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok(_service.GetAll());
        }

        [HttpGet("odata/Categories/{id}")]
        [EnableQuery]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(short id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound(new { message = $"Category with ID {id} not found." });
            return Ok(category);
        }

        [HttpPost("odata/Categories")]
        [Authorize(Roles = "Staff,Lecturer")]
        public async Task<IActionResult> Post([FromBody] Category category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _service.CreateAsync(category);
            return Created($"odata/Categories({created.CategoryID})", created);
        }

        [HttpPut("odata/Categories/{id}")]
        [Authorize(Roles = "Staff,Lecturer")]
        public async Task<IActionResult> Put(short id, [FromBody] Category category)
        {
            if (id != category.CategoryID) return BadRequest(new { message = "ID mismatch." });
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = await _service.UpdateAsync(category);
            return Ok(updated);
        }

        [HttpDelete("odata/Categories/{id}")]
        [Authorize(Roles = "Staff,Lecturer")]
        public async Task<IActionResult> Delete(short id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result)
                return BadRequest(new { message = "Cannot delete this category because it has associated news articles." });
            return NoContent();
        }
    }
}
