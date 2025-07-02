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
    public class PatientStaffRelationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseService _databaseService;

        public PatientStaffRelationController(ApplicationDbContext context, DatabaseService databaseService)
        {
            _context = context;
            _databaseService = databaseService;
        }

        // GET: api/PatientStaffRelation
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientStaffRelation>>> GetPatientStaffRelations()
        {
            return await _context.PatientStaffRelations.ToListAsync();
        }

        // GET: api/PatientStaffRelation/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetRelationsPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.PatientStaffRelations.CountAsync();
            var relations = await _context.PatientStaffRelations
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = relations,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/PatientStaffRelation/ByPatient/5
        [HttpGet("ByPatient/{patientId}")]
        public async Task<ActionResult<IEnumerable<PatientStaffRelation>>> GetPatientStaffRelationsByPatient(string patientId)
        {
            return await _context.PatientStaffRelations
                .Where(x => x.patient_id == patientId)
                .ToListAsync();
        }

        // GET: api/PatientStaffRelation/ByStaff/5
        [HttpGet("ByStaff/{staffId}")]
        public async Task<ActionResult<IEnumerable<PatientStaffRelation>>> GetRelationsByStaff(string staffId)
        {
            return await _context.PatientStaffRelations
                .Where(r => r.staff_id == staffId)
                .ToListAsync();
        }

        // GET: api/PatientStaffRelation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientStaffRelation>> GetPatientStaffRelation(int id)
        {
            var patientStaffRelation = await _context.PatientStaffRelations.FindAsync(id);

            if (patientStaffRelation == null)
            {
                return NotFound();
            }

            return patientStaffRelation;
        }

        // POST: api/PatientStaffRelation
        [HttpPost]
        public async Task<ActionResult<PatientStaffRelation>> PostPatientStaffRelation(PatientStaffRelation patientStaffRelation)
        {
            _context.PatientStaffRelations.Add(patientStaffRelation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPatientStaffRelation", new { id = patientStaffRelation.relation_id }, patientStaffRelation);
        }

        // PUT: api/PatientStaffRelation/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatientStaffRelation(int id, PatientStaffRelation patientStaffRelation)
        {
            if (id != patientStaffRelation.relation_id)
            {
                return BadRequest();
            }

            _context.Entry(patientStaffRelation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientStaffRelationExists(id))
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

        // DELETE: api/PatientStaffRelation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientStaffRelation(int id)
        {
            var patientStaffRelation = await _context.PatientStaffRelations.FindAsync(id);
            if (patientStaffRelation == null)
            {
                return NotFound();
            }

            _context.PatientStaffRelations.Remove(patientStaffRelation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PatientStaffRelationExists(int id)
        {
            return _context.PatientStaffRelations.Any(e => e.relation_id == id);
        }
    }
}