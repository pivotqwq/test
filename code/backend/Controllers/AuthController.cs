using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.username.ToLower() == request.Username.ToLower() &&
                u.password == request.Password);
                var token = GenerateJwtToken(user.Id.ToString());

                if (user != null)
                {
                    return Ok(new { success = true, message = "Login successful" ,token = token, username = request.Username,userId = user.Id });
                }

                return Ok(new { success = false, message = "Invalid credentials" });
            }catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 验证用户名是否已存在
            if (await _context.Users.AnyAsync(u => u.username.ToLower() == request.Username.ToLower()))
            {
                return BadRequest(new { success = false, message = "Username already exists" });
            }

            // 验证邮箱是否已存在
            if (await _context.Users.AnyAsync(u => u.email.ToLower() == request.Email.ToLower()))
            {
                return BadRequest(new { success = false, message = "Email already exists" });
            }

            // 创建新用户
            var newUser = new User
            {
                username = request.Username,
                password = request.Password, // 注意：实际项目中密码应该哈希存储
                email = request.Email,
                name = "未设置",
                profession = "未设置",
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Registration successful" });
        }
        private string GenerateJwtToken(string userId)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, userId), // 用户ID存入标准sub字段
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId) // 也可以存入NameIdentifier
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("is-admin/{userId}")]
        public async Task<IActionResult> IsAdmin(string userId)
        {
            var isAdmin = await _context.Admins
                .AnyAsync(a => a.user_id == userId && a.is_admin);

            return Ok(new { IsAdmin = isAdmin });
        }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}