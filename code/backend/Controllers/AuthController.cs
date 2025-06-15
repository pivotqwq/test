using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.username.ToLower() == request.Username.ToLower() &&
                u.password == request.Password);

            if (user != null)
            {
                return Ok(new { success = true, message = "Login successful" });
            }

            return Ok(new { success = false, message = "Invalid credentials" });
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
                phone = "默认值",
                urlBase64 = "Base64pic",
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Registration successful" });
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