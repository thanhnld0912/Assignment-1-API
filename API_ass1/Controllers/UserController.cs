using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Services;
using System.Security.Claims;
namespace API_ass1.Controllers
{
    [Route("/user")]
    public class UserController : Microsoft.AspNetCore.OData.Routing.Controllers.ODataController
    {


        private readonly AccountService _accountService;
        private readonly ArticleService _articleService;
        private readonly IMapper _mapper;

        public UserController(AccountService accountService, IMapper mapper, ArticleService articleService)
        {
            _accountService = accountService;
            _articleService = articleService;
            _mapper = mapper;
        }
        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] SystemAccount updated)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != updated.AccountId.ToString())
                return Unauthorized("You can only edit your own profile.");

            await _accountService.UpdateAccount(updated);
            return Ok("Profile updated.");
        }

        [HttpGet("GetMyArticles")]
        [Authorize]

        public async Task<IActionResult> GetMyArticles()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var articles = await _articleService.GetArticlesByUserId(short.Parse(userId));
            return Ok(articles);
        }

    }

}
