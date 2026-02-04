using CMSWebClient.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMSWebClient.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly WebClientContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<AppUser> _passwordHasher;

        public AuthorizationController(WebClientContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<AppUser>();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            // Find user in database
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Username == request.Username && !u.IsDeleted);

            if (user == null)
            {
                Console.WriteLine($"User not found: {request.Username}");
                return Unauthorized(new { message = "Invalid credentials" });
            }

            Console.WriteLine($"User found: {user.Username}, ID: {user.Id}");

            // Verify password hash
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

            Console.WriteLine($"Password verification result: {result}");

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return Ok(new
            {
                accessToken = token,
                userId = user.Id,
                username = user.Username,
                firstName = user.FirstName,
                lastName = user.LastName,
                email = user.Email
            });
        }

        [HttpGet("TestDbConnection")]
        public async Task<IActionResult> TestDbConnection()
        {
            try
            {
                var users = await _context.AppUsers.ToListAsync();

                return Ok(new
                {
                    message = "Database connected successfully",
                    userCount = users.Count,
                    users = users.Select(u => new
                    {
                        id = u.Id,
                        username = u.Username,
                        passwordHashPrefix = u.Password?.Substring(0, Math.Min(30, u.Password.Length)),
                        isDeleted = u.IsDeleted
                    })
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    error = ex.Message,
                    innerError = ex.InnerException?.Message
                });
            }
        }

        private string GenerateJwtToken(AppUser user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}