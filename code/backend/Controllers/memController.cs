using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static backend.Data.ApplicationDbContext;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class memController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseService _databaseService;

        public memController(ApplicationDbContext context, DatabaseService databaseService)
        {
            _context = context;
            _databaseService = databaseService;
        }

        // GET: api/mem/myMemos
        [HttpGet("myMemos")]
        public async Task<IActionResult> GetMemos(string userId)
        {
            var memos = await _context.Memos
                .Where(n => n.userid.Equals(userId) && n.isdone == 0).ToListAsync();

            if (!memos.Any())
            {
                return Ok(new
                {
                    code = 200,
                    message = "备忘录为空"
                });
            }
            var memos2 = memos.OrderByDescending(m => m.created_at);
                

            return Ok(new
            {
                code = 200,
                data = memos2
            });
        }

        [HttpPost("addMemos")]
        [Obsolete]
        public async Task<IActionResult> PostMemo([FromBody] Memo memo)
        {
            // 1. 模型验证
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    code = 400,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            // 2. 自动生成ID
            if (memo.id != 0) memo.id = 0;

            // 3. 保存数据
            try
            {
                _context.Memos.Add(memo);
                int affected = await _context.SaveChangesAsync();

                if (affected == 0)
                {
                    throw new Exception("数据未保存，影响行数为0");
                }

                return Ok(new
                {
                    code = 200,
                    data = memo,
                    affectedRows = affected
                });
            }
            catch (DbUpdateException ex)
            {
                // 捕获数据库异常
                var sqlEx = ex.InnerException;
                return StatusCode(500, new
                {
                    code = 500,
                    error = "数据库保存失败",
                    detail = sqlEx?.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = 500,
                    error = "服务器内部错误",
                    detail = ex.Message
                });
            }
        }

        // PUT: api/mem/changeMemos
        [HttpPut("changeMemos")]
        public async Task<IActionResult> PutMemo(int id, [FromBody] dataChange msg)
        {
            var memo = await _context.Memos
                .FirstOrDefaultAsync(n => n.id == id);

            if (memo==null)
            {
                return BadRequest(new
                {
                    code = 400,
                    message = "ID不匹配"
                });
            }

            memo.title = msg.title;
            memo.content = msg.content;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemoExists(id))
                {
                    return NotFound(new
                    {
                        code = 404,
                        findid = id,
                        message = "未找到对应备忘录"
                    });
                }

                throw;
            }

            return Ok(new
            {
                code = 200,
                data = memo
            });
        }

        // DELETE: api/mem/del
        [HttpDelete("del")]
        public async Task<IActionResult> DeleteMemo(int id)
        {
            var memo = await _context.Memos.FindAsync(id);
            if (memo == null)
            {
                return NotFound(new
                {
                    code = 404,
                    message = "未找到对应备忘录"
                });
            }

            _context.Memos.Remove(memo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                code = 200,
                message = "删除成功"
            });
        }

        private bool MemoExists(int id)
        {
            return _context.Memos.Any(e => e.id == id);
        }

        public class dataChange
        {
            public string title { get; set; }
            public string content { get; set; }
        }
    }
}
