using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenomicDatasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenomicDatasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/GenomicDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenomicData>>> GetGenomicDatas()
        {
            return await _context.GenomicDatas.ToListAsync();
        }

        // GET: api/GenomicDatas/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetGenomicDatasPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.GenomicDatas.CountAsync();
            var genomicDatas = await _context.GenomicDatas
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = genomicDatas,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/GenomicDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GenomicData>> GetGenomicData(int id)
        {
            var genomicData = await _context.GenomicDatas.FindAsync(id);

            if (genomicData == null)
            {
                return NotFound();
            }

            return genomicData;
        }

        // GET: api/GenomicDatas/BySpecimen/5
        [HttpGet("BySpecimen/{specimenId}")]
        public async Task<ActionResult<IEnumerable<GenomicData>>> GetGenomicDatasBySpecimen(string specimenId)
        {
            return await _context.GenomicDatas
                .Where(g => g.specimen_id == specimenId)
                .ToListAsync();
        }

        // POST: api/GenomicDatas
        [HttpPost]
        public async Task<ActionResult<GenomicData>> PostGenomicData(GenomicData genomicData)
        {
            _context.GenomicDatas.Add(genomicData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGenomicData", new { id = genomicData.data_id }, genomicData);
        }

        // PUT: api/GenomicDatas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenomicData(int id, GenomicData genomicData)
        {
            if (id != genomicData.data_id)
            {
                return BadRequest();
            }

            _context.Entry(genomicData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenomicDataExists(id))
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

        // DELETE: api/GenomicDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenomicData(int id)
        {
            var genomicData = await _context.GenomicDatas.FindAsync(id);
            if (genomicData == null)
            {
                return NotFound();
            }

            _context.GenomicDatas.Remove(genomicData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GenomicDataExists(int id)
        {
            return _context.GenomicDatas.Any(e => e.data_id == id);
        }
    }
}