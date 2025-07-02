using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationRecordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseService _databaseService;

        public MedicationRecordController(ApplicationDbContext context, DatabaseService databaseService)
        {
            _context = context;
            _databaseService = databaseService;
        }

        // GET: api/MedicationRecord
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicationRecord>>> GetMedicationRecords()
        {
            return await _context.MedicationRecords
                .OrderByDescending(m => m.start_date)
                .ToListAsync();
        }

        // GET: api/MedicationRecord/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetMedicationRecordsPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.MedicationRecords.CountAsync();
            var medications = await _context.MedicationRecords
                .OrderByDescending(m => m.start_date)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = medications,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/MedicationRecord/ActiveByPatient/5
        [HttpGet("ActiveByPatient/{patientId}")]
        public async Task<ActionResult<IEnumerable<MedicationRecord>>> GetActiveMedications(string patientId)
        {
            return await _context.MedicationRecords
                .Where(m => m.patient_id == patientId && (m.end_date == null || m.end_date >= DateTime.Today))
                .OrderBy(m => m.drug_name)
                .ToListAsync();
        }

        // GET: api/MedicationRecord/ByPatient/5
        [HttpGet("ByPatient/{patientId}")]
        public async Task<ActionResult<IEnumerable<MedicationRecord>>> GetMedicationsByPatient(string patientId)
        {
            return await _context.MedicationRecords
                .Where(m => m.patient_id == patientId)
                .OrderByDescending(m => m.start_date)
                .ToListAsync();
        }

        // GET: api/MedicationRecord/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicationRecord>> GetMedicationRecord(int id)
        {
            var medicationRecord = await _context.MedicationRecords.FindAsync(id);

            if (medicationRecord == null)
            {
                return NotFound();
            }

            return medicationRecord;
        }

        // POST: api/MedicationRecord
        [HttpPost]
        public async Task<ActionResult<MedicationRecord>> PostMedication(MedicationRecord medication)
        {
            _context.MedicationRecords.Add(medication);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMedicationRecord", new { id = medication.medication_id }, medication);
        }

        // PUT: api/MedicationRecord/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicationRecord(int id, MedicationRecord medicationRecord)
        {
            if (id != medicationRecord.medication_id)
            {
                return BadRequest();
            }

            _context.Entry(medicationRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicationRecordExists(id))
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

        // DELETE: api/MedicationRecord/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicationRecord(int id)
        {
            var medicationRecord = await _context.MedicationRecords.FindAsync(id);
            if (medicationRecord == null)
            {
                return NotFound();
            }

            _context.MedicationRecords.Remove(medicationRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MedicationRecordExists(int id)
        {
            return _context.MedicationRecords.Any(e => e.medication_id == id);
        }
    }
}