using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Models.DTO;
using Models.Models;
using Services;
using System.Security.Claims;

namespace API_ass1.Controllers
{
    [Authorize(Roles = "1")]
    [Route("odata/[controller]")]
    public class ArticlesController : ODataController
    {
        private readonly ArticleService _articleService;
        private readonly TagService _tagService;
        private readonly IMapper _mapper;

        public ArticlesController(IMapper mapper, ArticleService articleService, TagService tagService)
        {
            _mapper = mapper;
            _articleService = articleService;
            _tagService = tagService;
        }

        //[EnableQuery]
        //[HttpGet("list")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetList()
        //{
        //    var articles = await _articleService.GetNewsArticles();
        //    return Ok(articles);
        //}
        [EnableQuery]
        [HttpGet("list")] // Swagger
        [AllowAnonymous]
        public async Task<IActionResult> GetList()
        {
            var articles = await _articleService.GetNewsArticles();
            return Ok(articles.AsQueryable());
        }

        //[EnableQuery]
        //[HttpGet] // GET /odata/Articles
        //[AllowAnonymous]
        //public async Task<IActionResult> Get()
        //{
        //    return await GetList(); // gọi lại hàm GetList()
        //}
        [EnableQuery]
        [HttpGet("{key}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById([FromODataUri] string key)
        {
            var article = await _articleService.GetArticleById(key);
            if (article == null)
                return NotFound();

            return Ok(article);
        }


        // ✅ POST /odata/Articles
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewsArticleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            var article = _mapper.Map<NewsArticle>(dto);
            article.CreatedById = short.Parse(userId);
            article.UpdatedById = article.CreatedById;

            await _articleService.AddNewsArticle(article);
            return Created(article);
        }

        // ✅ PUT /odata/Articles(1)
        [Authorize]
        [HttpPut("{key}")]
        public async Task<IActionResult> Put([FromODataUri] string key, [FromBody] NewsArticleDTO dto)
        {
            if (key != dto.NewsArticleId)
                return BadRequest("ID mismatch.");

            var existing = await _articleService.GetArticleById(key);
            if (existing == null)
                return NotFound("Article not found.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");

            // Cập nhật các trường
            existing.NewsTitle = dto.NewsTitle;
            existing.Headline = dto.Headline;
            existing.NewsContent = dto.NewsContent;
            existing.NewsSource = dto.NewsSource;
            existing.CategoryId = dto.CategoryId;
            existing.NewsStatus = dto.NewsStatus;
            existing.UpdatedById = short.Parse(userId);
            existing.ModifiedDate = DateTime.UtcNow;

            await _articleService.UpdateArticle(existing);
            return Updated(existing);
        }

        // ✅ DELETE /odata/Articles(1)
        [Authorize]
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromODataUri] string key)
        {
            var article = await _articleService.GetArticleById(key);
            if (article == null)
                return NotFound("Article not found.");

            await _articleService.DeleteArticle(article);
            return NoContent();
        }
    }
}
