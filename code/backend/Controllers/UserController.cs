using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseService _databaseService;

        public UserController(ApplicationDbContext context, DatabaseService databaseService)
        {
            _context = context;
            _databaseService = databaseService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                // 基本错误检验
                if (_context.Users == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new {code = 500, message = "数据库上下文未正确初始化" });
                }

                // 获取用户和管理员信息
                var usersWithRoles = await (from user in _context.Users
                                          join admin in _context.Admins on user.Id equals admin.user_id into adminGroup
                                          from adminInfo in adminGroup.DefaultIfEmpty()
                                          select new {
                                              user.Id,
                                              user.username,
                                              user.email,
                                              user.phone,
                                              user.name,
                                              user.profession,
                                              role = adminInfo != null && adminInfo.is_admin ? "admin" : "user",
                                              created_at = DateTime.UtcNow // 模拟创建时间，实际应该从数据库获取
                                          }).AsNoTracking().ToListAsync();

                return Ok(usersWithRoles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { code = 500, message = "处理请求时发生内部错误" });
            }
        }

        // GET: api/User/allUsers
        [HttpGet("allUsers")]
        public async Task<IActionResult> GetUserBy(short page = 1,short limit = 12)
        {
            var allusers = await _context.Users.AsQueryable().ToListAsync();
            var users = await _context.Users.AsQueryable().Skip((page - 1) * limit)
                    .Take(limit).ToListAsync();
                

            if (users == null)
            {
                return NotFound(new { code = 404, message = "用户未找到" });
            }

            int totCount = allusers.Count();

            return Ok(new
            {
                code = 200,
                tot = Math.Max(1,totCount / limit + (totCount % limit == 0 ? 0:1)),
                message = users,
                all = allusers
            });
        }

        // GET: api/User/findUser
        [HttpGet("findUser")]
        public async Task<IActionResult> GetUserByUsername([FromQuery]string username)
        {
            var user = await _context.Users
                .Where(u => u.username == username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { code = 404, message = "用户未找到" });
            }

            return Ok(new { code = 200, data = user });
        }

        // POST: api/User/change
        [HttpPost("change")]
        public async Task<object> changeUses([FromBody]UserChange userData)
        {
            var user = await _context.Users
                .Where(u => u.username == userData.Username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { code = 404, message = "用户未找到" });
            }

            user.name = userData.Name;
            user.email = userData.Email;
            user.profession = userData.Profession;
            user.phone = userData.Phone;
            
            await _context.SaveChangesAsync();
            return Ok(new { code = 200, data = user });
        }

        [HttpPost("uploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file, [FromQuery] string username)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { code = 400, message = "未上传文件" });
            }

            if (file.Length > 2 * 1024 * 1024)
            {
                return BadRequest(new { code = 400, message = "文件大小不能超过2MB" });
            }

            var allowedExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { code = 400, message = "仅支持PNG、JPG、JPEG、GIF格式" });
            }

            var user = await _context.Users
            .Where(u => u.username == username)
            .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { code = 404, message = "用户不存在" });
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                var base64String = Convert.ToBase64String(fileBytes);

                user.urlBase64 = base64String;

                await _context.SaveChangesAsync();
            }

            return Ok(new { code = 200, message = "头像更新成功" , avatarPath = user.urlBase64});
        }

        // POST: api/User/setAdmin
        [HttpPost("setAdmin")]
        public async Task<IActionResult> SetUserAsAdmin([FromBody] SetAdminRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.username == request.Username);
                if (user == null)
                {
                    return NotFound(new { code = 404, message = "用户不存在" });
                }

                // 检查是否已经是管理员
                var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.user_id == user.Id);
                if (existingAdmin != null)
                {
                    existingAdmin.is_admin = true;
                }
                else
                {
                    // 创建新的管理员记录
                    var newAdmin = new backend.Data.Admin
                    {
                        user_id = user.Id,
                        is_admin = true
                    };
                    _context.Admins.Add(newAdmin);
                }

                await _context.SaveChangesAsync();
                return Ok(new { code = 200, message = $"用户 {request.Username} 已设置为管理员" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { code = 500, message = "设置管理员失败" });
            }
        }

        // POST: api/User/removeAdmin
        [HttpPost("removeAdmin")]
        public async Task<IActionResult> RemoveAdminRole([FromBody] SetAdminRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.username == request.Username);
                if (user == null)
                {
                    return NotFound(new { code = 404, message = "用户不存在" });
                }

                var adminRecord = await _context.Admins.FirstOrDefaultAsync(a => a.user_id == user.Id);
                if (adminRecord != null)
                {
                    adminRecord.is_admin = false;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { code = 200, message = $"已移除用户 {request.Username} 的管理员权限" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { code = 500, message = "移除管理员权限失败" });
            }
        }

        public class UserChange
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Name { get; set; }
            public string? Profession { get; set; }
        }

        public class SetAdminRequest
        {
            public string Username { get; set; }
        }

    }
}
