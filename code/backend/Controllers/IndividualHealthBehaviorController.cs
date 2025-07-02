using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndividualHealthBehaviorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IndividualHealthBehaviorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/IndividualHealthBehavior
        [HttpGet]
        public async Task<ActionResult<IEnumerable<IndividualHealthBehavior>>> GetIndividualHealthBehaviors()
        {
            return await _context.IndividualHealthBehaviors.ToListAsync();
        }

        // GET: api/IndividualHealthBehavior/ByPatient/5
        [HttpGet("ByPatient/{patientId}")]
        public async Task<ActionResult<IEnumerable<IndividualHealthBehavior>>> GetIndividualHealthBehaviorsByPatient(string patientId)
        {
            return await _context.IndividualHealthBehaviors
                .Where(h => h.patient_id == patientId)
                .ToListAsync();
        }

        // GET: api/IndividualHealthBehavior/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IndividualHealthBehavior>> GetIndividualHealthBehavior(string id)
        {
            var individualHealthBehavior = await _context.IndividualHealthBehaviors.FindAsync(id);

            if (individualHealthBehavior == null)
            {
                return NotFound();
            }

            return individualHealthBehavior;
        }

        // PUT: api/IndividualHealthBehavior/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIndividualHealthBehavior(string id, [FromBody] JsonElement requestData)
        {
            try
            {
                // 查找现有记录
                var existingRecord = await _context.IndividualHealthBehaviors.FindAsync(id);
                if (existingRecord == null)
                {
                    return NotFound();
                }

                // 从JsonElement中安全提取数据并更新字段，根据字段类型正确转换
                if (requestData.TryGetProperty("patient_id", out var pidProp))
                    existingRecord.patient_id = pidProp.GetString();
                
                if (requestData.TryGetProperty("household_id", out var hidProp))
                    existingRecord.household_id = hidProp.GetString();
                
                if (requestData.TryGetProperty("diet_pattern", out var dpProp))
                    existingRecord.diet_pattern = dpProp.GetString();
                
                if (requestData.TryGetProperty("vitamin_d_level", out var vdProp))
                    existingRecord.vitamin_d_level = vdProp.ValueKind != JsonValueKind.Null ? (decimal?)vdProp.GetDecimal() : null;
                
                if (requestData.TryGetProperty("sun_exposure", out var seProp))
                    existingRecord.sun_exposure = seProp.GetBoolean();
                
                if (requestData.TryGetProperty("vaccination_status", out var vsProp))
                    existingRecord.vaccination_status = vsProp.GetBoolean();
                
                if (requestData.TryGetProperty("antibiotic_usage_frequency", out var aufProp))
                    existingRecord.antibiotic_usage_frequency = aufProp.GetString();
                
                if (requestData.TryGetProperty("early_life_medication", out var elmProp))
                    existingRecord.early_life_medication = elmProp.GetString();
                
                if (requestData.TryGetProperty("smoke_exposure", out var smProp))
                    existingRecord.smoke_exposure = smProp.GetBoolean();
                
                if (requestData.TryGetProperty("investigator_id", out var iiProp))
                    existingRecord.investigator_id = iiProp.GetString();

                await _context.SaveChangesAsync();

                return Ok(new { message = "个人健康行为数据更新成功", data = existingRecord });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "更新个人健康行为数据失败: " + ex.Message });
            }
        }

        // POST: api/IndividualHealthBehavior
        [HttpPost]
        public async Task<ActionResult<IndividualHealthBehavior>> PostIndividualHealthBehavior([FromBody] JsonElement requestData)
        {
            try
            {
                // 从JsonElement中安全提取数据，根据字段类型正确转换
                var individualHealthBehavior = new IndividualHealthBehavior
                {
                    individual_id = requestData.TryGetProperty("individual_id", out var iidProp) ? iidProp.GetString() : null,
                    patient_id = requestData.TryGetProperty("patient_id", out var pidProp) ? pidProp.GetString() : null,
                    household_id = requestData.TryGetProperty("household_id", out var hidProp) ? hidProp.GetString() : null,
                    diet_pattern = requestData.TryGetProperty("diet_pattern", out var dpProp) ? dpProp.GetString() : null,
                    vitamin_d_level = requestData.TryGetProperty("vitamin_d_level", out var vdProp) && vdProp.ValueKind != JsonValueKind.Null
                        ? (decimal?)vdProp.GetDecimal() : null,
                    sun_exposure = requestData.TryGetProperty("sun_exposure", out var seProp) ? seProp.GetBoolean() : false,
                    vaccination_status = requestData.TryGetProperty("vaccination_status", out var vsProp) ? vsProp.GetBoolean() : false,
                    antibiotic_usage_frequency = requestData.TryGetProperty("antibiotic_usage_frequency", out var aufProp) ? aufProp.GetString() : null,
                    early_life_medication = requestData.TryGetProperty("early_life_medication", out var elmProp) ? elmProp.GetString() : null,
                    smoke_exposure = requestData.TryGetProperty("smoke_exposure", out var smProp) ? smProp.GetBoolean() : false,
                    investigator_id = requestData.TryGetProperty("investigator_id", out var iiProp) ? iiProp.GetString() : null
                };

                _context.IndividualHealthBehaviors.Add(individualHealthBehavior);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetIndividualHealthBehavior", new { id = individualHealthBehavior.individual_id }, individualHealthBehavior);
            }
            catch (DbUpdateException)
            {
                var iid = requestData.TryGetProperty("individual_id", out var iidProp) ? iidProp.GetString() : null;
                if (!string.IsNullOrEmpty(iid) && IndividualHealthBehaviorExists(iid))
                {
                    return Conflict(new { message = "个人健康行为数据ID已存在" });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "添加个人健康行为数据失败: " + ex.Message });
            }
        }

        // DELETE: api/IndividualHealthBehavior/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIndividualHealthBehavior(string id)
        {
            var individualHealthBehavior = await _context.IndividualHealthBehaviors.FindAsync(id);
            if (individualHealthBehavior == null)
            {
                return NotFound();
            }

            _context.IndividualHealthBehaviors.Remove(individualHealthBehavior);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IndividualHealthBehaviorExists(string id)
        {
            return _context.IndividualHealthBehaviors.Any(e => e.individual_id == id);
        }
    }
}