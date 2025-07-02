using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhysicalExaminationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseService _databaseService;

        public PhysicalExaminationController(ApplicationDbContext context, DatabaseService databaseService)
        {
            _context = context;
            _databaseService = databaseService;
        }

        // GET: api/PhysicalExamination
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhysicalExamination>>> GetPhysicalExaminations()
        {
            return await _context.PhysicalExaminations.OrderBy(x => x.exam_date).ToListAsync();
        }

        // GET: api/PhysicalExamination/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetPhysicalExaminationsPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.PhysicalExaminations.CountAsync();
            var examinations = await _context.PhysicalExaminations
                .OrderByDescending(e => e.exam_date)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = examinations,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/PhysicalExamination/ByPatient/5
        [HttpGet("ByPatient/{patientId}")]
        public async Task<ActionResult<IEnumerable<PhysicalExamination>>> GetPhysicalExaminationsByPatient(string patientId)
        {
            return await _context.PhysicalExaminations
                .Where(x => x.patient_id == patientId)
                .OrderBy(x => x.exam_date)
                .ToListAsync();
        }

        // GET: api/PhysicalExamination/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PhysicalExamination>> GetPhysicalExamination(int id)
        {
            var physicalExamination = await _context.PhysicalExaminations.FindAsync(id);

            if (physicalExamination == null)
            {
                return NotFound();
            }

            return physicalExamination;
        }

        // POST: api/PhysicalExamination
        [HttpPost]
        public async Task<ActionResult<PhysicalExamination>> PostPhysicalExamination(PhysicalExamination physicalExamination)
        {
            _context.PhysicalExaminations.Add(physicalExamination);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPhysicalExamination", new { id = physicalExamination.exam_id }, physicalExamination);
        }

        // PUT: api/PhysicalExamination/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPhysicalExamination(int id, PhysicalExamination physicalExamination)
        {
            if (id != physicalExamination.exam_id)
            {
                return BadRequest();
            }

            _context.Entry(physicalExamination).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhysicalExaminationExists(id))
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

        // DELETE: api/PhysicalExamination/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhysicalExamination(int id)
        {
            var physicalExamination = await _context.PhysicalExaminations.FindAsync(id);
            if (physicalExamination == null)
            {
                return NotFound();
            }

            _context.PhysicalExaminations.Remove(physicalExamination);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PhysicalExaminationExists(int id)
        {
            return _context.PhysicalExaminations.Any(e => e.exam_id == id);
        }
    }
}