using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowUpRecordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FollowUpRecordController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/FollowUpRecord
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetFollowUpRecords()
        {
            var followUpRecords = await _context.FollowUpRecords
                .Join(_context.PatientBasicInfos,
                    followUp => followUp.patient_id,
                    patient => patient.patient_id,
                    (followUp, patient) => new
                    {
                        followup_id = followUp.followup_id,
                        patient_id = followUp.patient_id,
                        patient_name = patient.name ?? "未知患者",
                        followup_date = followUp.followup_date,
                        symptom_improvement = followUp.symptom_improvement,
                        adverse_effects = followUp.adverse_effects,
                        act_score = followUp.act_score
                    })
                .OrderBy(x => x.followup_date)
                .ToListAsync();

            return Ok(followUpRecords);
        }

        // GET: api/FollowUpRecord/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetFollowUpRecordsPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.FollowUpRecords.CountAsync();
            var followUps = await _context.FollowUpRecords
                .Join(_context.PatientBasicInfos,
                    followUp => followUp.patient_id,
                    patient => patient.patient_id,
                    (followUp, patient) => new
                    {
                        followup_id = followUp.followup_id,
                        patient_id = followUp.patient_id,
                        patient_name = patient.name ?? "未知患者",
                        followup_date = followUp.followup_date,
                        symptom_improvement = followUp.symptom_improvement,
                        adverse_effects = followUp.adverse_effects,
                        act_score = followUp.act_score
                    })
                .OrderByDescending(f => f.followup_date)
                .OrderBy(f => f.act_score)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = followUps,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/FollowUpRecord/ByPatient/5
        [HttpGet("ByPatient/{patientId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetFollowUpRecordsByPatient(string patientId)
        {
            var followUpRecords = await _context.FollowUpRecords
                .Join(_context.PatientBasicInfos,
                    followUp => followUp.patient_id,
                    patient => patient.patient_id,
                    (followUp, patient) => new
                    {
                        followup_id = followUp.followup_id,
                        patient_id = followUp.patient_id,
                        patient_name = patient.name ?? "未知患者",
                        followup_date = followUp.followup_date,
                        symptom_improvement = followUp.symptom_improvement,
                        adverse_effects = followUp.adverse_effects,
                        act_score = followUp.act_score
                    })
                .Where(x => x.patient_id == patientId)
                .OrderBy(x => x.followup_date)
                .ToListAsync();

            return Ok(followUpRecords);
        }

        // GET: api/FollowUpRecord/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FollowUpRecord>> GetFollowUpRecord(int id)
        {
            var followUpRecord = await _context.FollowUpRecords.FindAsync(id);

            if (followUpRecord == null)
            {
                return NotFound();
            }

            return followUpRecord;
        }

        // POST: api/FollowUpRecord
        [HttpPost]
        public async Task<ActionResult<FollowUpRecord>> PostFollowUpRecord(FollowUpRecord followUpRecord)
        {
            try
            {
                // 验证患者是否存在
                var patientExists = await _context.PatientBasicInfos.AnyAsync(p => p.patient_id == followUpRecord.patient_id);
                if (!patientExists)
                {
                    return BadRequest(new { message = $"患者ID {followUpRecord.patient_id} 不存在" });
                }

                _context.FollowUpRecords.Add(followUpRecord);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetFollowUpRecord", new { id = followUpRecord.followup_id }, followUpRecord);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "添加随访记录失败: " + ex.Message });
            }
        }

        // PUT: api/FollowUpRecord/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFollowUpRecord(int id, FollowUpRecord followUpRecord)
        {
            if (id != followUpRecord.followup_id)
            {
                return BadRequest();
            }

            _context.Entry(followUpRecord).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FollowUpRecordExists(id))
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

        // DELETE: api/FollowUpRecord/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFollowUpRecord(int id)
        {
            try
            {
                var followUpRecord = await _context.FollowUpRecords.FindAsync(id);
                if (followUpRecord == null)
                {
                    return NotFound(new { message = $"随访记录ID {id} 不存在" });
                }

                _context.FollowUpRecords.Remove(followUpRecord);
                await _context.SaveChangesAsync();

                return Ok(new { message = "随访记录删除成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "删除随访记录失败: " + ex.Message });
            }
        }

        private bool FollowUpRecordExists(int id)
        {
            return _context.FollowUpRecords.Any(e => e.followup_id == id);
        }
    }
}