using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.DTO;
using Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_ass1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly IConfiguration _configuration;

        public AuthController(AccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
            _configuration = configuration;
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // For stateless JWT, just inform frontend to remove the token
            return Ok("Logged out.");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var adminEmail = _configuration["DefaultAdminAccount:Email"];
            var adminPassword = _configuration["DefaultAdminAccount:Password"];

            if (loginDto.Email == adminEmail && loginDto.Password == adminPassword)
            {
                return Ok(GenerateJwtToken("0", "Admin", adminEmail, "admin"));
            }

            var user = await _accountService.GetSystemAccountByGmailPass(loginDto.Email, loginDto.Password);
            if (user == null) return Unauthorized("Invalid email or password");

            string role = user.AccountRole switch
            {
                1 => "staff",
                2 => "lecturer",
                _ => "unknown"
            };

            return Ok(GenerateJwtToken(user.AccountId.ToString(), user.AccountName, user.AccountEmail, role));
        }
        private string GenerateJwtToken(string id, string name, string email, string role)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, id),
        new Claim(ClaimTypes.Name, name),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }


}
