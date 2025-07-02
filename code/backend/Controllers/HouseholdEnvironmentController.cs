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
    public class HouseholdEnvironmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HouseholdEnvironmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/HouseholdEnvironment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HouseholdEnvironment>>> GetHouseholdEnvironments()
        {
            return await _context.HouseholdEnvironments.ToListAsync();
        }

        // GET: api/HouseholdEnvironment/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<HouseholdEnvironment>>> GetHouseholdEnvironmentsByPatient(string patientId)
        {
            return await _context.HouseholdEnvironments
                .Where(h => h.patient_id == patientId)
                .ToListAsync();
        }

        // GET: api/HouseholdEnvironment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HouseholdEnvironment>> GetHouseholdEnvironment(string id)
        {
            var householdEnvironment = await _context.HouseholdEnvironments.FindAsync(id);

            if (householdEnvironment == null)
            {
                return NotFound();
            }

            return householdEnvironment;
        }

        // PUT: api/HouseholdEnvironment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHouseholdEnvironment(string id, [FromBody] JsonElement requestData)
        {
            try
            {
                // 查找现有记录
                var existingRecord = await _context.HouseholdEnvironments.FindAsync(id);
                if (existingRecord == null)
                {
                    return NotFound();
                }

                // 从JsonElement中安全提取数据并更新字段，根据字段类型正确转换
                if (requestData.TryGetProperty("patient_id", out var pidProp))
                    existingRecord.patient_id = pidProp.GetString();
                
                if (requestData.TryGetProperty("residence_type", out var rtProp))
                    existingRecord.residence_type = rtProp.GetString();
                
                if (requestData.TryGetProperty("building_age", out var baProp))
                    existingRecord.building_age = baProp.ValueKind != JsonValueKind.Null ? (int?)baProp.GetInt32() : null;
                
                if (requestData.TryGetProperty("ventilation_quality", out var vqProp))
                    existingRecord.ventilation_quality = vqProp.GetString();
                
                if (requestData.TryGetProperty("indoor_pm25", out var pm25Prop))
                    existingRecord.indoor_pm25 = pm25Prop.ValueKind != JsonValueKind.Null ? (decimal?)pm25Prop.GetDecimal() : null;
                
                if (requestData.TryGetProperty("pet_exposure", out var peProp))
                    existingRecord.pet_exposure = peProp.GetBoolean();
                
                if (requestData.TryGetProperty("pet_type", out var ptProp))
                    existingRecord.pet_type = ptProp.GetString();
                
                if (requestData.TryGetProperty("bedding_material", out var bmProp))
                    existingRecord.bedding_material = bmProp.GetString();
                
                if (requestData.TryGetProperty("record_date", out var rdProp) && rdProp.ValueKind != JsonValueKind.Null)
                    existingRecord.record_date = DateTime.Parse(rdProp.GetString()).ToUniversalTime();
                
                if (requestData.TryGetProperty("investigator_id", out var iiProp))
                    existingRecord.investigator_id = iiProp.GetString();

                await _context.SaveChangesAsync();

                return Ok(new { message = "家庭环境数据更新成功", data = existingRecord });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "更新家庭环境数据失败: " + ex.Message });
            }
        }

        // POST: api/HouseholdEnvironment
        [HttpPost]
        public async Task<ActionResult<HouseholdEnvironment>> PostHouseholdEnvironment([FromBody] JsonElement requestData)
        {
            try
            {
                // 从JsonElement中安全提取数据，根据字段类型正确转换
                var householdEnvironment = new HouseholdEnvironment
                {
                    household_id = requestData.TryGetProperty("household_id", out var hidProp) ? hidProp.GetString() : null,
                    patient_id = requestData.TryGetProperty("patient_id", out var pidProp) ? pidProp.GetString() : null,
                    residence_type = requestData.TryGetProperty("residence_type", out var rtProp) ? rtProp.GetString() : null,
                    building_age = requestData.TryGetProperty("building_age", out var baProp) && baProp.ValueKind != JsonValueKind.Null
                        ? (int?)baProp.GetInt32() : null,
                    ventilation_quality = requestData.TryGetProperty("ventilation_quality", out var vqProp) ? vqProp.GetString() : null,
                    indoor_pm25 = requestData.TryGetProperty("indoor_pm25", out var pm25Prop) && pm25Prop.ValueKind != JsonValueKind.Null
                        ? (decimal?)pm25Prop.GetDecimal() : null,
                    pet_exposure = requestData.TryGetProperty("pet_exposure", out var peProp) ? peProp.GetBoolean() : false,
                    pet_type = requestData.TryGetProperty("pet_type", out var ptProp) ? ptProp.GetString() : null,
                    bedding_material = requestData.TryGetProperty("bedding_material", out var bmProp) ? bmProp.GetString() : null,
                    record_date = requestData.TryGetProperty("record_date", out var rdProp) && rdProp.ValueKind != JsonValueKind.Null
                        ? DateTime.Parse(rdProp.GetString()).ToUniversalTime()
                        : DateTime.UtcNow,
                    investigator_id = requestData.TryGetProperty("investigator_id", out var iiProp) ? iiProp.GetString() : null
                };

                _context.HouseholdEnvironments.Add(householdEnvironment);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetHouseholdEnvironment", new { id = householdEnvironment.household_id }, householdEnvironment);
            }
            catch (DbUpdateException)
            {
                var hid = requestData.TryGetProperty("household_id", out var hidProp) ? hidProp.GetString() : null;
                if (!string.IsNullOrEmpty(hid) && HouseholdEnvironmentExists(hid))
                {
                    return Conflict(new { message = "家庭环境数据ID已存在" });
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "添加家庭环境数据失败: " + ex.Message });
            }
        }

        // DELETE: api/HouseholdEnvironment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHouseholdEnvironment(string id)
        {
            var householdEnvironment = await _context.HouseholdEnvironments.FindAsync(id);
            if (householdEnvironment == null)
            {
                return NotFound();
            }

            _context.HouseholdEnvironments.Remove(householdEnvironment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HouseholdEnvironmentExists(string id)
        {
            return _context.HouseholdEnvironments.Any(e => e.household_id == id);
        }
    }
}