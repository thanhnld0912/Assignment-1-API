using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Services;

namespace API_ass1.Controllers
{
    //[Authorize(Roles = "staff")]
    [Route("odata/[controller]")]
    public class TagsController : ODataController
    {
        private readonly TagService _tagService;

        public TagsController(TagService tagService)
        {
            _tagService = tagService;
        }

        // ✅ GET /odata/Tags
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _tagService.GetTags();
            return Ok(tags);
        }

        // ✅ GET /odata/Tags/{key}
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> GetById([FromODataUri] int key)
        {
            var tag = await _tagService.GetTagById(key);
            return tag == null ? NotFound() : Ok(tag);
        }

        // ✅ POST /odata/Tags/AddTags
        [HttpPost("AddTags")]
        [Authorize]
        public async Task<IActionResult> AddTagsToArticle([FromQuery] string articleId, [FromBody] List<int> tagIds)
        {
            if (string.IsNullOrWhiteSpace(articleId) || tagIds == null || !tagIds.Any())
                return BadRequest("Article ID or Tag IDs are invalid");

            foreach (var tagId in tagIds)
            {
                await _tagService.AddTagToArticle(articleId, tagId);
            }

            return Ok("Tags added successfully.");
        }

        // ✅ POST /odata/Tags/SyncArticleTags
        [HttpPost("SyncArticleTags")]
        [Authorize]
        public async Task<IActionResult> SyncTags([FromQuery] string articleId, [FromBody] List<int> tagIds)
        {
            if (string.IsNullOrWhiteSpace(articleId))
                return BadRequest("Missing article ID");

            var currentTags = await _tagService.GetTagsByArticle(articleId);

            // Remove unchecked tags
            foreach (var existing in currentTags)
            {
                if (!tagIds.Contains(existing.TagId))
                    await _tagService.RemoveTagFromArticle(articleId, existing.TagId);
            }

            // Add new checked tags
            foreach (var tagId in tagIds)
            {
                if (!currentTags.Any(t => t.TagId == tagId))
                    await _tagService.AddTagToArticle(articleId, tagId);
            }

            return Ok("Tags synchronized.");
        }
    }
}
