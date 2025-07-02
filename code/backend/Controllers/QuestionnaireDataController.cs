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
    public class QuestionnaireDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionnaireDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/QuestionnaireData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionnaireData>>> GetQuestionnaireDatas()
        {
            return await _context.QuestionnaireDatas.ToListAsync();
        }

        // GET: api/QuestionnaireData/patient/{patientId}
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<QuestionnaireData>>> GetQuestionnaireDatasByPatient(string patientId)
        {
            return await _context.QuestionnaireDatas
                .Where(q => q.patient_id == patientId)
                .ToListAsync();
        }

        // GET: api/QuestionnaireData/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionnaireData>> GetQuestionnaireData(string id)
        {
            var questionnaireData = await _context.QuestionnaireDatas.FindAsync(id);

            if (questionnaireData == null)
            {
                return NotFound();
            }

            return questionnaireData;
        }

        // PUT: api/QuestionnaireData/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestionnaireData(string id, [FromBody] JsonElement requestData)
        {
            try
            {
                // 查找现有记录
                var existingRecord = await _context.QuestionnaireDatas.FindAsync(id);
                if (existingRecord == null)
                {
                    return NotFound();
                }

                // 从JsonElement中安全提取数据并更新字段
                if (requestData.TryGetProperty("patient_id", out var pidProp))
                    existingRecord.patient_id = pidProp.GetString();
                
                if (requestData.TryGetProperty("form_type", out var ftProp))
                    existingRecord.form_type = ftProp.GetString();
                
                if (requestData.TryGetProperty("fill_date", out var fdProp))
                    existingRecord.fill_date = fdProp.GetString();
                
                if (requestData.TryGetProperty("data_source", out var dsProp))
                    existingRecord.data_source = dsProp.GetString();
                
                if (requestData.TryGetProperty("investigator_id", out var iiProp))
                    existingRecord.investigator_id = iiProp.GetString();
                
                if (requestData.TryGetProperty("raw_data", out var rdProp))
                    existingRecord.raw_data = rdProp.GetString();
                
                if (requestData.TryGetProperty("create_time", out var ctProp) && ctProp.ValueKind != JsonValueKind.Null)
                    existingRecord.create_time = DateTime.Parse(ctProp.GetString()).ToUniversalTime();

                await _context.SaveChangesAsync();

                return Ok(new { message = "问卷调查数据更新成功", data = existingRecord });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "更新问卷调查数据失败: " + ex.Message });
            }
        }

        // POST: api/QuestionnaireData
        [HttpPost]
        public async Task<ActionResult<QuestionnaireData>> PostQuestionnaireData([FromBody] JsonElement requestData)
        {
            try
            {
                // 从JsonElement中安全提取数据
                var questionnaireData = new QuestionnaireData
                {
                    questionnaire_id = requestData.TryGetProperty("questionnaire_id", out var qidProp) && !qidProp.ValueEquals("") 
                        ? qidProp.GetString() 
                        : "Q" + DateTime.UtcNow.Ticks,
                    patient_id = requestData.TryGetProperty("patient_id", out var pidProp) ? pidProp.GetString() : null,
                    form_type = requestData.TryGetProperty("form_type", out var ftProp) ? ftProp.GetString() : null,
                    fill_date = requestData.TryGetProperty("fill_date", out var fdProp) ? fdProp.GetString() : null,
                    data_source = requestData.TryGetProperty("data_source", out var dsProp) ? dsProp.GetString() : null,
                    investigator_id = requestData.TryGetProperty("investigator_id", out var iiProp) ? iiProp.GetString() : null,
                    raw_data = requestData.TryGetProperty("raw_data", out var rdProp) ? rdProp.GetString() : null,
                    create_time = requestData.TryGetProperty("create_time", out var ctProp) && ctProp.ValueKind != JsonValueKind.Null
                        ? DateTime.Parse(ctProp.GetString()).ToUniversalTime()
                        : DateTime.UtcNow
                };

                _context.QuestionnaireDatas.Add(questionnaireData);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetQuestionnaireData", new { id = questionnaireData.questionnaire_id }, questionnaireData);
            }
            catch (DbUpdateException ex)
            {
                var qid = requestData.TryGetProperty("questionnaire_id", out var qidProp) ? qidProp.GetString() : null;
                if (!string.IsNullOrEmpty(qid) && QuestionnaireDataExists(qid))
                {
                    return Conflict(new { message = "问卷调查数据ID已存在" });
                }
                else
                {
                    return BadRequest(new { message = "添加问卷调查数据失败: " + ex.Message });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "添加问卷调查数据失败: " + ex.Message });
            }
        }

        // DELETE: api/QuestionnaireData/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestionnaireData(string id)
        {
            try
            {
                var questionnaireData = await _context.QuestionnaireDatas.FindAsync(id);
                if (questionnaireData == null)
                {
                    return NotFound(new { message = $"问卷调查数据ID {id} 不存在" });
                }

                _context.QuestionnaireDatas.Remove(questionnaireData);
                await _context.SaveChangesAsync();

                return Ok(new { message = "问卷调查数据删除成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "删除问卷调查数据失败: " + ex.Message });
            }
        }

        private bool QuestionnaireDataExists(string id)
        {
            return _context.QuestionnaireDatas.Any(e => e.questionnaire_id == id);
        }
    }
}