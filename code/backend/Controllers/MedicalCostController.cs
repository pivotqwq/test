using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalCostController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MedicalCostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MedicalCost
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalCost>>> GetMedicalCosts()
        {
            return await _context.MedicalCosts.OrderBy(x => x.cost_date).ToListAsync();
        }

        // GET: api/MedicalCost/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetMedicalCostsPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.MedicalCosts.CountAsync();
            var costs = await _context.MedicalCosts
                .OrderByDescending(c => c.cost_date)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = costs,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/MedicalCost/ByPatient/5
        [HttpGet("ByPatient/{patientId}")]
        public async Task<ActionResult<IEnumerable<MedicalCost>>> GetMedicalCostsByPatient(string patientId)
        {
            return await _context.MedicalCosts
                .Where(x => x.patient_id == patientId)
                .OrderBy(x => x.cost_date)
                .ToListAsync();
        }

        // GET: api/MedicalCost/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalCost>> GetMedicalCost(int id)
        {
            var medicalCost = await _context.MedicalCosts.FindAsync(id);

            if (medicalCost == null)
            {
                return NotFound();
            }

            return medicalCost;
        }

        // POST: api/MedicalCost
        [HttpPost]
        public async Task<ActionResult<MedicalCost>> PostMedicalCost(MedicalCost medicalCost)
        {
            _context.MedicalCosts.Add(medicalCost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedicalCost", new { id = medicalCost.cost_id }, medicalCost);
        }

        // PUT: api/MedicalCost/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicalCost(int id, MedicalCost medicalCost)
        {
            if (id != medicalCost.cost_id)
            {
                return BadRequest();
            }

            _context.Entry(medicalCost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicalCostExists(id))
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

        // DELETE: api/MedicalCost/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalCost(int id)
        {
            var medicalCost = await _context.MedicalCosts.FindAsync(id);
            if (medicalCost == null)
            {
                return NotFound();
            }

            _context.MedicalCosts.Remove(medicalCost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicalCostExists(int id)
        {
            return _context.MedicalCosts.Any(e => e.cost_id == id);
        }
    }
}