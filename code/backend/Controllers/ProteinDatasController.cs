using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProteinDatasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProteinDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ProteinDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProteinData>>> GetProteinDatas()
        {
            return await _context.ProteinDatas.ToListAsync();
        }

        // GET: api/ProteinDatas/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetProteinDatasPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.ProteinDatas.CountAsync();
            var proteinDatas = await _context.ProteinDatas
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = proteinDatas,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/ProteinDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProteinData>> GetProteinData(int id)
        {
            var proteinData = await _context.ProteinDatas.FindAsync(id);

            if (proteinData == null)
            {
                return NotFound();
            }

            return proteinData;
        }

        // GET: api/ProteinDatas/BySpecimen/5
        [HttpGet("BySpecimen/{specimenId}")]
        public async Task<ActionResult<IEnumerable<ProteinData>>> GetProteinDatasBySpecimen(string specimenId)
        {
            return await _context.ProteinDatas
                .Where(p => p.specimen_id == specimenId)
                .ToListAsync();
        }

        // POST: api/ProteinDatas
        [HttpPost]
        public async Task<ActionResult<ProteinData>> PostProteinData(ProteinData proteinData)
        {
            _context.ProteinDatas.Add(proteinData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProteinData", new { id = proteinData.data_id }, proteinData);
        }

        // PUT: api/ProteinDatas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProteinData(int id, ProteinData proteinData)
        {
            if (id != proteinData.data_id)
            {
                return BadRequest();
            }

            _context.Entry(proteinData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProteinDataExists(id))
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

        // DELETE: api/ProteinDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProteinData(int id)
        {
            var proteinData = await _context.ProteinDatas.FindAsync(id);
            if (proteinData == null)
            {
                return NotFound();
            }

            _context.ProteinDatas.Remove(proteinData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProteinDataExists(int id)
        {
            return _context.ProteinDatas.Any(e => e.data_id == id);
        }
    }
}
