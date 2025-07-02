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
    public class PatientBasicInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientBasicInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PatientBasicInfo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientBasicInfo>>> GetPatientBasicInfos()
        {
            return await _context.PatientBasicInfos.ToListAsync();
        }

        // GET: api/PatientBasicInfo/paged?page=1&limit=10&name=xxx
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetPatientBasicInfosPaged(int page = 1, int limit = 10, string? name = null, string? medicalRecordNo = null)
        {
            var query = _context.PatientBasicInfos.AsQueryable();

            // 添加搜索条件
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(medicalRecordNo))
            {
                query = query.Where(p => p.patient_id.Contains(medicalRecordNo));
            }

            var totalCount = await query.CountAsync();
            var patients = await query
                .OrderBy(p => p.patient_id) // 添加排序以确保分页结果一致
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = patients,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/PatientBasicInfo/search?keyword=xxx
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchPatientBasicInfos(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { code = 400, message = "搜索关键词不能为空" });
            }

            var patients = await _context.PatientBasicInfos
                .Where(p => p.name.Contains(keyword) || p.patient_id.Contains(keyword))
                .ToListAsync();

            return Ok(new { code = 200, data = patients });
        }

        // GET: api/PatientBasicInfo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientBasicInfo>> GetPatientBasicInfo(string id)
        {
            var patientBasicInfo = await _context.PatientBasicInfos.FindAsync(id);

            if (patientBasicInfo == null)
            {
                return NotFound();
            }

            return patientBasicInfo;
        }

        // 新增：获取患者完整关联数据
        [HttpGet("{id}/complete")]
        public async Task<ActionResult<object>> GetPatientCompleteInfo(string id)
        {
            // 首先尝试在PatientBasicInfo表中查找（使用patient_id）
            var patient = await _context.PatientBasicInfos.FindAsync(id);
            
            // 如果没找到，返回未找到错误（不再从patients表查找）
            
            if (patient == null)
            {
                return NotFound(new { message = "患者不存在" });
            }

            // 使用患者的patient_id来查询关联数据
            var patientId = patient.patient_id;

            // 获取所有关联数据
            var followUps = await _context.FollowUpRecords
                .Where(f => f.patient_id == patientId)
                .OrderByDescending(f => f.followup_date)
                .ToListAsync();

            var questionnaires = await _context.QuestionnaireDatas
                .Where(q => q.patient_id == patientId)
                .OrderByDescending(q => q.create_time)
                .ToListAsync();

            var medications = await _context.MedicationRecords
                .Where(m => m.patient_id == patientId)
                .OrderByDescending(m => m.start_date)
                .ToListAsync();

            var physicalExams = await _context.PhysicalExaminations
                .Where(p => p.patient_id == patientId)
                .OrderByDescending(p => p.exam_date)
                .ToListAsync();

            var specimens = await _context.SpecimenInfos
                .Where(s => s.patient_id == patientId)
                .OrderByDescending(s => s.collection_date)
                .ToListAsync();

            var labTests = await _context.LabTests
                .Where(l => l.patient_id == patientId)
                .ToListAsync();

            var diagnoses = await _context.Diagnoses
                .Where(d => d.patient_id == patientId)
                .ToListAsync();

            var medicalCosts = await _context.MedicalCosts
                .Where(m => m.patient_id == patientId)
                .OrderByDescending(m => m.cost_date)
                .ToListAsync();

            var householdEnv = await _context.HouseholdEnvironments
                .Where(h => h.patient_id == patientId)
                .OrderByDescending(h => h.record_date)
                .ToListAsync();

            var healthBehaviors = await _context.IndividualHealthBehaviors
                .Where(i => i.patient_id == patientId)
                .ToListAsync();

            // 获取标本相关数据
            var specimenIds = specimens.Select(s => s.specimen_id).ToList();
            var specimenQualities = await _context.SpecimenQualities
                .Where(q => specimenIds.Contains(q.specimen_id))
                .ToListAsync();

            var genomicData = await _context.GenomicDatas
                .Where(g => specimenIds.Contains(g.specimen_id))
                .ToListAsync();

            var proteinData = await _context.ProteinDatas
                .Where(p => specimenIds.Contains(p.specimen_id))
                .ToListAsync();

            var clinicalData = await _context.ClinicalDatas
                .Where(c => specimenIds.Contains(c.specimen_id))
                .ToListAsync();

            return Ok(new
            {
                patient = patient,
                followUps = followUps,
                questionnaires = questionnaires,
                medications = medications,
                physicalExams = physicalExams,
                specimens = specimens,
                labTests = labTests,
                diagnoses = diagnoses,
                medicalCosts = medicalCosts,
                householdEnvironment = householdEnv,
                healthBehaviors = healthBehaviors,
                specimenQualities = specimenQualities,
                genomicData = genomicData,
                proteinData = proteinData,
                clinicalData = clinicalData,
                summary = new
                {
                    totalFollowUps = followUps.Count,
                    totalQuestionnaires = questionnaires.Count,
                    totalMedications = medications.Count,
                    totalExams = physicalExams.Count,
                    totalSpecimens = specimens.Count,
                    totalLabTests = labTests.Count,
                    totalDiagnoses = diagnoses.Count,
                    totalCosts = medicalCosts.Sum(m => m.amount),
                    lastFollowUp = followUps.FirstOrDefault()?.followup_date,
                    lastQuestionnaire = questionnaires.FirstOrDefault()?.create_time
                }
            });
        }

        // 新增：获取患者数据统计
        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetPatientsStatistics()
        {
            var totalPatients = await _context.PatientBasicInfos.CountAsync();
            var maleCount = await _context.PatientBasicInfos.CountAsync(p => p.gender == "M");
            var femaleCount = await _context.PatientBasicInfos.CountAsync(p => p.gender == "F");
            
            var ageGroups = await _context.PatientBasicInfos
                .GroupBy(p => p.age_at_diagnosi >= 1 && p.age_at_diagnosi < 3 ? "1-3岁" :
                            p.age_at_diagnosi >= 3 && p.age_at_diagnosi < 5 ? "3-5岁" :
                            p.age_at_diagnosi >= 5 ? "5岁以上" : "未知")
                .Select(g => new { AgeGroup = g.Key, Count = g.Count() })
                .ToListAsync();

            var residenceTypes = await _context.PatientBasicInfos
                .GroupBy(p => p.residence_type == "1" ? "城市" :
                            p.residence_type == "2" ? "城镇" :
                            p.residence_type == "3" ? "农村" : "未知")
                .Select(g => new { ResidenceType = g.Key, Count = g.Count() })
                .ToListAsync();

            var followUpCount = await _context.FollowUpRecords.CountAsync();
            var questionnaireCount = await _context.QuestionnaireDatas.CountAsync();
            var specimenCount = await _context.SpecimenInfos.CountAsync();

            return Ok(new
            {
                totalPatients = totalPatients,
                genderDistribution = new { male = maleCount, female = femaleCount },
                ageDistribution = ageGroups,
                residenceDistribution = residenceTypes,
                dataStatistics = new
                {
                    totalFollowUps = followUpCount,
                    totalQuestionnaires = questionnaireCount,
                    totalSpecimens = specimenCount,
                    averageFollowUpsPerPatient = totalPatients > 0 ? (double)followUpCount / totalPatients : 0,
                    averageQuestionnairesPerPatient = totalPatients > 0 ? (double)questionnaireCount / totalPatients : 0
                }
            });
        }

        // PUT: api/PatientBasicInfo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatientBasicInfo(string id, PatientBasicInfo patientBasicInfo)
        {
            try
            {
                // 验证输入参数
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "患者ID不能为空" });
                }

                if (patientBasicInfo == null)
                {
                    return BadRequest(new { message = "患者信息不能为空" });
                }

                // 先获取现有的患者记录
                var existingPatient = await _context.PatientBasicInfos.FindAsync(id);
                if (existingPatient == null)
                {
                    return NotFound(new { message = $"未找到ID为 {id} 的患者" });
                }

                // 记录原始值用于调试
                Console.WriteLine($"更新患者 {id}:");
                Console.WriteLine($"原始birth_date: {existingPatient.birth_date}");
                Console.WriteLine($"新birth_date: {patientBasicInfo.birth_date}");

                // 更新字段，保持create_time不变
                if (!string.IsNullOrEmpty(patientBasicInfo.name))
                    existingPatient.name = patientBasicInfo.name;
                
                if (!string.IsNullOrEmpty(patientBasicInfo.gender))
                    existingPatient.gender = patientBasicInfo.gender;
                
                // 修复日期比较逻辑
                if (patientBasicInfo.birth_date != default(DateTime) && patientBasicInfo.birth_date.Year > 1900)
                {
                    // 处理DateTime时区问题
                    if (patientBasicInfo.birth_date.Kind == DateTimeKind.Unspecified)
                    {
                        existingPatient.birth_date = DateTime.SpecifyKind(patientBasicInfo.birth_date, DateTimeKind.Utc);
                    }
                    else if (patientBasicInfo.birth_date.Kind == DateTimeKind.Local)
                    {
                        existingPatient.birth_date = patientBasicInfo.birth_date.ToUniversalTime();
                    }
                    else
                    {
                        existingPatient.birth_date = patientBasicInfo.birth_date;
                    }
                }
                
                if (patientBasicInfo.age_at_diagnosi.HasValue)
                    existingPatient.age_at_diagnosi = patientBasicInfo.age_at_diagnosi;
                
                if (!string.IsNullOrEmpty(patientBasicInfo.residence_type))
                    existingPatient.residence_type = patientBasicInfo.residence_type;
                
                if (!string.IsNullOrEmpty(patientBasicInfo.allergy_history))
                    existingPatient.allergy_history = patientBasicInfo.allergy_history;
                
                if (!string.IsNullOrEmpty(patientBasicInfo.phone))
                    existingPatient.phone = patientBasicInfo.phone;
                
                existingPatient.update_time = DateTime.UtcNow; // 自动设置更新时间(UTC)

                // 保存前验证
                _context.Entry(existingPatient).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                return Ok(new { message = "患者信息更新成功", data = existingPatient });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PatientBasicInfoExists(id))
                {
                    return NotFound(new { message = $"患者 {id} 不存在" });
                }
                else
                {
                    Console.WriteLine($"并发错误: {ex.Message}");
                    Console.WriteLine($"内部异常: {ex.InnerException?.Message}");
                    return StatusCode(500, new { message = $"并发更新错误: {ex.Message}" });
                }
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"数据库更新错误: {ex.Message}");
                Console.WriteLine($"内部异常: {ex.InnerException?.Message}");
                return StatusCode(500, new { message = $"数据库更新错误: {ex.InnerException?.Message ?? ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新错误: {ex.Message}");
                Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
                return StatusCode(500, new { message = $"更新患者信息时发生错误: {ex.Message}" });
            }
        }

        // POST: api/PatientBasicInfo
        [HttpPost]
        public async Task<ActionResult<PatientBasicInfo>> PostPatientBasicInfo(PatientBasicInfo patientBasicInfo)
        {
            try
            {
                Console.WriteLine($"接收到的患者数据: {JsonSerializer.Serialize(patientBasicInfo)}");
                
                // 验证输入参数
                if (patientBasicInfo == null)
                {
                    return BadRequest(new { message = "患者信息不能为空" });
                }

                // 验证必需字段
                if (string.IsNullOrWhiteSpace(patientBasicInfo.name))
                {
                    return BadRequest(new { message = "患者姓名不能为空" });
                }

                if (string.IsNullOrWhiteSpace(patientBasicInfo.gender))
                {
                    return BadRequest(new { message = "患者性别不能为空" });
                }

                if (string.IsNullOrWhiteSpace(patientBasicInfo.residence_type))
                {
                    return BadRequest(new { message = "居住类型不能为空" });
                }

                // 过敏史可以为空，如果为空则设置为"无"
                if (string.IsNullOrWhiteSpace(patientBasicInfo.allergy_history))
                {
                    patientBasicInfo.allergy_history = "无";
                }

                // 验证出生日期（如果提供的话）
                if (patientBasicInfo.birth_date != default(DateTime) && patientBasicInfo.birth_date.Year < 1900)
                {
                    return BadRequest(new { message = "出生日期格式不正确" });
                }

                // 处理DateTime字段，确保为UTC时间
                if (patientBasicInfo.birth_date != default(DateTime))
                {
                    // 如果birth_date不是UTC时间，则转换为UTC
                    if (patientBasicInfo.birth_date.Kind == DateTimeKind.Unspecified)
                    {
                        patientBasicInfo.birth_date = DateTime.SpecifyKind(patientBasicInfo.birth_date, DateTimeKind.Utc);
                    }
                    else if (patientBasicInfo.birth_date.Kind == DateTimeKind.Local)
                    {
                        patientBasicInfo.birth_date = patientBasicInfo.birth_date.ToUniversalTime();
                    }
                }

                // 自动生成病患ID
                patientBasicInfo.patient_id = await GenerateNextPatientId();
                patientBasicInfo.create_time = DateTime.UtcNow;
                patientBasicInfo.update_time = null; // 新建时不设置更新时间

                Console.WriteLine($"准备保存的患者数据: {JsonSerializer.Serialize(patientBasicInfo)}");

                _context.PatientBasicInfos.Add(patientBasicInfo);
                await _context.SaveChangesAsync();

                Console.WriteLine($"患者创建成功，ID: {patientBasicInfo.patient_id}");

                return CreatedAtAction("GetPatientBasicInfo", new { id = patientBasicInfo.patient_id }, new 
                {
                    message = "患者创建成功",
                    data = patientBasicInfo
                });
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"数据库更新错误: {ex.Message}");
                Console.WriteLine($"内部异常: {ex.InnerException?.Message}");
                return StatusCode(500, new { message = $"数据库保存错误: {ex.InnerException?.Message ?? ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"创建患者错误: {ex.Message}");
                Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
                return StatusCode(500, new { message = $"创建患者信息时发生错误: {ex.Message}" });
            }
        }

        // 生成下一个病患ID的私有方法
        private async Task<string> GenerateNextPatientId()
        {
            try
            {
                // 获取所有以P开头的病患ID
                var existingIds = await _context.PatientBasicInfos
                    .Where(p => p.patient_id.StartsWith("P"))
                    .Select(p => p.patient_id)
                    .ToListAsync();

                int maxNumber = 0;

                // 提取数字部分并找到最大值
                foreach (var id in existingIds)
                {
                    if (id.Length >= 2 && id.StartsWith("P"))
                    {
                        string numberPart = id.Substring(1); // 去掉P前缀
                        if (int.TryParse(numberPart, out int number))
                        {
                            maxNumber = Math.Max(maxNumber, number);
                        }
                    }
                }

                // 生成下一个ID，格式为P + 3位数字（如P001, P020等）
                int nextNumber = maxNumber + 1;
                return $"P{nextNumber:D3}";
            }
            catch (Exception ex)
            {
                // 如果出现异常，从P001开始
                return "P001";
            }
        }

        // DELETE: api/PatientBasicInfo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientBasicInfo(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "患者ID不能为空" });
                }

                var patientBasicInfo = await _context.PatientBasicInfos.FindAsync(id);
                if (patientBasicInfo == null)
                {
                    return NotFound(new { message = $"未找到ID为 {id} 的患者" });
                }

                _context.PatientBasicInfos.Remove(patientBasicInfo);
                await _context.SaveChangesAsync();

                return Ok(new { message = $"患者 {patientBasicInfo.name} (ID: {id}) 已成功删除" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"删除患者信息时发生错误: {ex.Message}" });
            }
        }

        private bool PatientBasicInfoExists(string id)
        {
            return _context.PatientBasicInfos.Any(e => e.patient_id == id);
        }
    }
}