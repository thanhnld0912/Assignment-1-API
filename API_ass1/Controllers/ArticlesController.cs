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
    [Authorize(Roles = "staff,1")]
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
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var articles = await _articleService.GetNewsArticles();
            return Ok(articles);
        }
        [EnableQuery]
        public async Task<IActionResult> Get([FromODataUri] string key)
        {
            var article = await _articleService.GetArticleById(key);
            return article == null ? NotFound() : Ok(article);
        }
        [EnableQuery]
        [Authorize]
        public async Task<IActionResult> Put([FromRoute] string key, [FromBody] NewsArticleDTO dto)
        {
            if (key != dto.NewsArticleId)
                return BadRequest("ID mismatch.");

            var article = await _articleService.GetArticleById(key);
            if (article == null)
                return NotFound("Article not found.");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User not authenticated");

            // Update fields
            article.NewsTitle = dto.NewsTitle;
            article.Headline = dto.Headline;
            article.NewsContent = dto.NewsContent;
            article.NewsSource = dto.NewsSource;
            article.CategoryId = dto.CategoryId;
            article.NewsStatus = dto.NewsStatus;
            article.UpdatedById = short.Parse(userIdClaim);
            article.ModifiedDate = DateTime.UtcNow;
            await _articleService.UpdateArticle(article);
            return NoContent();
        }
        [Authorize]
        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] NewsArticleDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized("User not authenticated");

            var article = _mapper.Map<NewsArticle>(dto);
            article.NewsArticleId = dto.NewsArticleId;
            article.CreatedById = short.Parse(userIdClaim);
            article.UpdatedById = article.CreatedById;

            await _articleService.AddNewsArticle(article); // Save article without tags first

            return Created(article); // Return 201 Created
        }
        [EnableQuery]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] string key)
        {
            var article = await _articleService.GetArticleById(key);
            if (article == null)
                return NotFound("Article not found.");

            await _articleService.DeleteArticle(article);
            return NoContent();
        }



    }
}
