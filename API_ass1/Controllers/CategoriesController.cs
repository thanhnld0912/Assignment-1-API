using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Models.Models;
using Services;

namespace API_ass1.Controllers
{
    //[Authorize(Roles = "staff")]
    [Route("odata/[controller]")]
    public class CategoriesController : ODataController
    {
        private readonly CategoryService _cateService;

        public CategoriesController(CategoryService cateService)
        {
            _cateService = cateService;
        }

        // ✅ GET /odata/Categories
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _cateService.GetCategories();
            return Ok(categories);
        }

        // ✅ GET /odata/Categories/{key}
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetById([FromODataUri] short key)
        {
            var cate = await _cateService.GetCategoryById(key);
            return cate == null ? NotFound() : Ok(cate);
        }

        // ✅ POST /odata/Categories
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Category newCategory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _cateService.AddCategory(newCategory);
            return Created(newCategory);
        }

        // ✅ PUT /odata/Categories/{key}
        [HttpPut("{key}")]
        public async Task<IActionResult> Put([FromODataUri] short key, [FromBody] Category model)
        {
            if (key != model.CategoryId)
                return BadRequest("Mismatched category ID");

            await _cateService.UpdateCategory(model);
            return Updated(model);
        }

        // ✅ DELETE /odata/Categories/{key}
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromODataUri] short key)
        {
            var cate = await _cateService.GetCategoryById(key);
            if (cate == null)
                return NotFound();

            await _cateService.DeleteCategory(cate);
            return NoContent();
        }
    }
}
