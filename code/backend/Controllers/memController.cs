using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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

        // GET: api/memos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Memo>>> GetMemos()
        {
            return await _context.Memos.OrderByDescending(m => m.CreatedAt).ToListAsync();
        }

        // GET: api/memos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Memo>> GetMemo(int id)
        {
            var memo = await _context.Memos.FindAsync(id);

            if (memo == null)
            {
                return NotFound();
            }

            return memo;
        }

        // POST: api/memos
        [HttpPost]
        public async Task<ActionResult<Memo>> PostMemo(Memo memo)
        {
            _context.Memos.Add(memo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMemo), new { id = memo.Id }, memo);
        }

        // PUT: api/memos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMemo(int id, Memo memo)
        {
            if (id != memo.Id)
            {
                return BadRequest();
            }

            _context.Entry(memo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/memos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMemo(int id)
        {
            var memo = await _context.Memos.FindAsync(id);
            if (memo == null)
            {
                return NotFound();
            }

            _context.Memos.Remove(memo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MemoExists(int id)
        {
            return _context.Memos.Any(e => e.Id == id);
        }

        public class Memo
        {
            public int id { get; set; }
            public string title { get; set; }
            public string content { get; set; }
            public DateTime createdAt { get; set; } = DateTime.Now;
        }

    }
}
