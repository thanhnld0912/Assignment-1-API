using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Models.Models;
using Services;

namespace API_ass1.Controllers
{
    public class AccountsController : ODataController
    {
        private readonly AccountService _accountService;
        private readonly ArticleService _articleService;
        private readonly IMapper _mapper;

        public AccountsController(AccountService accountService, IMapper mapper, ArticleService articleService)
        {
            _accountService = accountService;
            _articleService = articleService;
            _mapper = mapper;
        }

        // ✅ GET /odata/Accounts
        [EnableQuery]
        [HttpGet] // cần để Swagger biết HTTP method
        public async Task<IActionResult> Get()
        {
            var accounts = await _accountService.GetSystemAccounts();
            return Ok(accounts);
        }

        // ✅ GET /odata/Accounts(1)
        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> Get([FromODataUri] short key)
        {
            var acc = await _accountService.GetAccountById(key);
            if (acc == null)
                return NotFound();

            return Ok(acc);
        }

        // ✅ POST /odata/Accounts
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SystemAccount newAccount)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _accountService.CreateAccount(newAccount);
            return Created(newAccount);
        }

        // ✅ PUT /odata/Accounts(1)
        [HttpPut("{key}")]
        public async Task<IActionResult> Put([FromODataUri] short key, [FromBody] SystemAccount updatedAccount)
        {
            if (key != updatedAccount.AccountId)
                return BadRequest("Key mismatch");

            var existing = await _accountService.GetAccountById(key);
            if (existing == null)
                return NotFound();

            await _accountService.UpdateAccount(updatedAccount);
            return Updated(updatedAccount);
        }

        // ✅ DELETE /odata/Accounts(1)
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete([FromODataUri] short key)
        {
            var existing = await _accountService.GetAccountById(key);
            if (existing == null)
                return NotFound();

            await _accountService.DeleteAccount(existing);
            return NoContent();
        }
    }
}
