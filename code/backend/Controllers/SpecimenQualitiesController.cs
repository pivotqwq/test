using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecimenQualitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpecimenQualitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/SpecimenQualities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpecimenQuality>>> GetSpecimenQualities()
        {
            return await _context.SpecimenQualities.ToListAsync();
        }

        // GET: api/SpecimenQualities/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetSpecimenQualitiesPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.SpecimenQualities.CountAsync();
            var qualities = await _context.SpecimenQualities
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = qualities,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/SpecimenQualities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecimenQuality>> GetSpecimenQuality(int id)
        {
            var specimenQuality = await _context.SpecimenQualities.FindAsync(id);

            if (specimenQuality == null)
            {
                return NotFound();
            }

            return specimenQuality;
        }

        // GET: api/SpecimenQualities/BySpecimen/5
        [HttpGet("BySpecimen/{specimenId}")]
        public async Task<ActionResult<IEnumerable<SpecimenQuality>>> GetSpecimenQualitiesBySpecimen(string specimenId)
        {
            return await _context.SpecimenQualities
                .Where(sq => sq.specimen_id == specimenId)
                .ToListAsync();
        }

        // POST: api/SpecimenQualities
        [HttpPost]
        public async Task<ActionResult<SpecimenQuality>> PostSpecimenQuality(SpecimenQuality specimenQuality)
        {
            _context.SpecimenQualities.Add(specimenQuality);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecimenQuality", new { id = specimenQuality.quality_id }, specimenQuality);
        }

        // PUT: api/SpecimenQualities/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecimenQuality(int id, SpecimenQuality specimenQuality)
        {
            if (id != specimenQuality.quality_id)
            {
                return BadRequest();
            }

            _context.Entry(specimenQuality).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpecimenQualityExists(id))
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

        // DELETE: api/SpecimenQualities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecimenQuality(int id)
        {
            var specimenQuality = await _context.SpecimenQualities.FindAsync(id);
            if (specimenQuality == null)
            {
                return NotFound();
            }

            _context.SpecimenQualities.Remove(specimenQuality);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SpecimenQualityExists(int id)
        {
            return _context.SpecimenQualities.Any(e => e.quality_id == id);
        }
    }
}