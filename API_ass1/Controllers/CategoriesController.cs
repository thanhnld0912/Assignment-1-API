using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Models.Models;
using Services;

namespace API_ass1.Controllers
{
    //[Authorize(Roles = "staff")]
    [Microsoft.AspNetCore.Mvc.Route("odata/[controller]")]
    public class CategoriesController : ODataController
    {
        private readonly CategoryService _cateService;

        public CategoriesController(CategoryService cateService)
        {
            _cateService = cateService;
        }
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var categories = await _cateService.GetCategories();
            return Ok(categories);
        }

        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] short key)
        {
            var cate = await _cateService.GetCategoryById(key);
            return cate == null ? NotFound() : Ok(cate);
        }

        public async Task<IActionResult> Post([FromBody] Category newCategory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _cateService.AddCategory(newCategory);
            return Created(newCategory);
        }
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromODataUri] short key)
        {
            var cate = await _cateService.GetCategoryById(key);
            if (cate == null)
                return NotFound();
            await _cateService.DeleteCategory(cate);
            return NoContent();
        }
        [HttpPut("{key}")]
        public async Task<IActionResult> Put([FromODataUri] short key, [FromBody] Category model)
        {
            if (key != model.CategoryId)
                return BadRequest();

            await _cateService.UpdateCategory(model);
            return Updated(model);
        }

    }
}
