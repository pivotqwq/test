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

                var users = await _context.Users
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(new { code = 200, message = users });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { code = 500,message = "处理请求时发生内部错误" });
            }
        }

        // GET: api/User/{username}
        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsername([FromQuery]string username)
        {
            var user = await _context.Users
                .Where(u => u.username == username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { code = 404, message = "用户未找到" });
            }

            return Ok(new
            {
                code = 200,
                data = new
                {
                    id = user.Id,
                    username = user.username,
                    email = user.email,
                    phone = user.phone,
                    urlBase64 = user.urlBase64
                }
            });
        }

        [HttpPost("change")]
        public async Task<object> changeUses([FromBody]User test)
        {
            return Ok(new { code = 200 });
        }

    }
}
