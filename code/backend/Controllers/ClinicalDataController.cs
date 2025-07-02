using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicalDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClinicalDataController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Insurance CRUD
        // 获取患者医保信息
        [HttpGet("insuranceInfo")]
        public async Task<IActionResult> GetInsuranceByPatient(string patientId)
        {
            var insurance = await _context.Insurance
                .Where(i => i.patient_id == patientId)
                .FirstOrDefaultAsync();

            if (insurance == null)
            {
                return NotFound(new { success = false, message = "Insurance record not found" });
            }

            return Ok(new { success = true, data = insurance });
        }

        // 添加或更新医保信息
        [HttpPost("insuranceAdd")]
        public async Task<IActionResult> AddOrUpdateInsurance([FromBody] InsuranceDto insuranceDto)
        {
            var existingInsurance = await _context.Insurance
                .Where(i => i.patient_id == insuranceDto.PatientId)
                .FirstOrDefaultAsync();

            if (existingInsurance != null)
            {
                // 更新现有记录
                existingInsurance.insurance_type = insuranceDto.InsuranceType;
                _context.Insurance.Update(existingInsurance);
            }
            else
            {
                // 添加新记录
                var newInsurance = new Insurance
                {
                    insurance_id = Guid.NewGuid().ToString(),
                    patient_id = insuranceDto.PatientId,
                    insurance_type = insuranceDto.InsuranceType
                };
                _context.Insurance.Add(newInsurance);
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Insurance record saved successfully" });
        }

        // 删除医保信息
        [HttpDelete("insuranceDel")]
        public async Task<IActionResult> DeleteInsurance(string insuranceId)
        {
            var insurance = await _context.Insurance.FindAsync(insuranceId);
            if (insurance == null)
            {
                return NotFound(new { success = false, message = "Insurance record not found" });
            }

            _context.Insurance.Remove(insurance);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Insurance record deleted successfully" });
        }
        #endregion

        #region Contacts CRUD
        // 获取患者联系人
        [HttpGet("contactsInfo")]
        public async Task<IActionResult> GetContactsByPatient(string patientId)
        {
            var contacts = await _context.Contacts
                .Where(c => c.patient_id == patientId)
                .ToListAsync();

            return Ok(new { success = true, data = contacts });
        }

        // 添加联系人
        [HttpPost("contactsAdd")]
        public async Task<IActionResult> AddContact([FromBody] ContactDto contactDto)
        {
            var newContact = new Contact
            {
                contact_id = Guid.NewGuid().ToString(),
                patient_id = contactDto.PatientId,
                name = contactDto.Name,
                contact_info = contactDto.ContactInfo
            };

            _context.Contacts.Add(newContact);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Contact added successfully", data = newContact });
        }

        // 更新联系人
        [HttpPut("contactsUpd")]
        public async Task<IActionResult> UpdateContact(string contactId, [FromBody] ContactDto contactDto)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return NotFound(new { success = false, message = "Contact not found" });
            }

            contact.name = contactDto.Name;
            contact.contact_info = contactDto.ContactInfo;

            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Contact updated successfully", data = contact });
        }

        // 删除联系人
        [HttpDelete("contactsDel")]
        public async Task<IActionResult> DeleteContact(string contactId)
        {
            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return NotFound(new { success = false, message = "Contact not found" });
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Contact deleted successfully" });
        }
        #endregion

        #region Medical Histories CRUD
        // 获取患者既往病史
        [HttpGet("medical-historiesInfo")]
        public async Task<IActionResult> GetMedicalHistoriesByPatient(string patientId)
        {
            var histories = await _context.MedicalHistories
                .Where(m => m.patient_id == patientId)
                .FirstOrDefaultAsync();

            return Ok(new { success = true, data = histories });
        }

        // 更新既往病史
        [HttpPut("medical-historiesUpd")]
        public async Task<IActionResult> UpdateMedicalHistory(string patientId, [FromBody] MedicalHistoryDto historyDto)
        {
            var history = await _context.MedicalHistories
                .Where(m => m.patient_id == patientId)
                .FirstOrDefaultAsync();

            if (history == null)
            {
                // 创建新记录
                history = new MedicalHistory
                {
                    history_id = Guid.NewGuid().ToString(),
                    patient_id = patientId,
                    allergy_history = historyDto.AllergyHistory
                };
                _context.MedicalHistories.Add(history);
            }
            else
            {
                // 更新现有记录
                history.allergy_history = historyDto.AllergyHistory;
                _context.MedicalHistories.Update(history);
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Medical history updated successfully", data = history });
        }
        #endregion

        #region Family Histories CRUD
        // 获取患者家族病史
        [HttpGet("family-historiesInfo")]
        public async Task<IActionResult> GetFamilyHistoriesByPatient(string patientId)
        {
            var histories = await _context.FamilyHistories
                .Where(f => f.patient_id == patientId)
                .FirstOrDefaultAsync();

            return Ok(new { success = true, data = histories });
        }

        // 更新家族病史
        [HttpPut("family-historiesInfo")]
        public async Task<IActionResult> UpdateFamilyHistory(string patientId, [FromBody] FamilyHistoryDto historyDto)
        {
            var history = await _context.FamilyHistories
                .Where(f => f.patient_id == patientId)
                .FirstOrDefaultAsync();

            if (history == null)
            {
                // 创建新记录
                history = new FamilyHistory
                {
                    family_history_id = Guid.NewGuid().ToString(),
                    patient_id = patientId,
                    allergy_history = historyDto.AllergyHistory
                };
                _context.FamilyHistories.Add(history);
            }
            else
            {
                // 更新现有记录
                history.allergy_history = historyDto.AllergyHistory;
                _context.FamilyHistories.Update(history);
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Family history updated successfully", data = history });
        }
        #endregion

        #region Lab Tests CRUD
        
        // 添加测试数据的方法（仅用于开发测试）
        [HttpPost("lab-tests/seed")]
        public async Task<IActionResult> SeedLabTestData()
        {
            try
            {
                // 检查是否已有数据
                var existingCount = await _context.LabTests.CountAsync();
                if (existingCount > 0)
                {
                    return Ok(new { success = true, message = $"数据库中已有 {existingCount} 条实验室数据" });
                }

                // 创建测试数据
                var testLabData = new List<LabTest>
                {
                    new LabTest
                    {
                        lab_id = Guid.NewGuid().ToString(),
                        patient_id = "P001",
                        item_name = "血常规",
                        exam_value = "正常",
                        exam_type = "blood"
                    },
                    new LabTest
                    {
                        lab_id = Guid.NewGuid().ToString(),
                        patient_id = "P002", 
                        item_name = "胸部CT",
                        exam_value = "轻度炎症",
                        exam_type = "imaging"
                    },
                    new LabTest
                    {
                        lab_id = Guid.NewGuid().ToString(),
                        patient_id = "P003",
                        item_name = "肺功能检查",
                        exam_value = "轻度受限",
                        exam_type = "pulmonary"
                    }
                };

                _context.LabTests.AddRange(testLabData);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    message = $"成功添加 {testLabData.Count} 条测试数据",
                    data = testLabData
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "添加测试数据失败: " + ex.Message 
                });
            }
        }
        // 获取实验室检查（可按患者ID筛选）
        [HttpGet("lab-testsInfo")]
        public async Task<IActionResult> GetLabTests([FromQuery] string? patientId, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            try
            {
                var query = _context.LabTests
                    .Join(_context.PatientBasicInfos,
                        labTest => labTest.patient_id,
                        patient => patient.patient_id,
                        (labTest, patient) => new
                        {
                            lab_id = labTest.lab_id,
                            patient_id = labTest.patient_id,
                            patient_name = patient.name ?? "未知患者",
                            item_name = labTest.item_name,
                            exam_value = labTest.exam_value,
                            exam_type = labTest.exam_type
                        });

                // 如果提供了患者ID，则按患者过滤
                if (!string.IsNullOrWhiteSpace(patientId))
                {
                    query = query.Where(l => l.patient_id == patientId);
                }

                // 获取总数
                int totalCount = await query.CountAsync();

                // 分页查询
                var labTests = await query
                    .OrderByDescending(l => l.lab_id) // 按ID倒序排列，最新的在前面
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                return Ok(new { 
                    success = true, 
                    data = labTests,
                    total = totalCount,
                    page = page,
                    limit = limit
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    message = "获取实验室检查数据失败: " + ex.Message 
                });
            }
        }

        // 添加实验室检查
        [HttpPost("lab-testsAdd")]
        public async Task<IActionResult> AddLabTest([FromBody] LabTestDto labTestDto)
        {
            var newLabTest = new LabTest
            {
                lab_id = Guid.NewGuid().ToString(),
                patient_id = labTestDto.PatientId,
                item_name = labTestDto.ItemName,
                exam_value = labTestDto.ExamValue,
                exam_type = labTestDto.ExamType
            };

            _context.LabTests.Add(newLabTest);
            await _context.SaveChangesAsync();

            // 如果有详情，添加详情
            if (labTestDto.ExamDetails != null)
            {
                if (labTestDto.ExamType == "imaging")
                {
                    var imagingDetail = new ImagingDetail
                    {
                        imaging_id = Guid.NewGuid().ToString(),
                        lab_id = newLabTest.lab_id,
                        exam_details = labTestDto.ExamDetails
                    };
                    _context.ImagingDetails.Add(imagingDetail);
                }
                else if (labTestDto.ExamType == "pulmonary")
                {
                    var pulmonaryDetail = new PulmonaryDetail
                    {
                        pulmonary_id = Guid.NewGuid().ToString(),
                        lab_id = newLabTest.lab_id,
                        exam_details = labTestDto.ExamDetails
                    };
                    _context.PulmonaryDetails.Add(pulmonaryDetail);
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true, message = "Lab test added successfully", data = newLabTest });
        }

        // 获取实验室检查详情
        [HttpGet("lab-tests/detailsInfo")]
        public async Task<IActionResult> GetLabTestDetails(string labId)
        {
            var labTest = await _context.LabTests.FindAsync(labId);
            if (labTest == null)
            {
                return NotFound(new { success = false, message = "Lab test not found" });
            }

            object details = null;
            if (labTest.exam_type == "imaging")
            {
                details = await _context.ImagingDetails
                    .Where(i => i.lab_id == labId)
                    .FirstOrDefaultAsync();
            }
            else if (labTest.exam_type == "pulmonary")
            {
                details = await _context.PulmonaryDetails
                    .Where(p => p.lab_id == labId)
                    .FirstOrDefaultAsync();
            }

            return Ok(new
            {
                success = true,
                data = new
                {
                    labTest = labTest,
                    details = details
                }
            });
        }

        // 删除实验室检查
        [HttpDelete("lab-testsDel")]
        public async Task<IActionResult> DeleteLabTest(string labId)
        {
            var labTest = await _context.LabTests.FindAsync(labId);
            if (labTest == null)
            {
                return NotFound(new { success = false, message = "Lab test not found" });
            }

            // 先删除关联的详情记录
            if (labTest.exam_type == "imaging")
            {
                var imagingDetail = await _context.ImagingDetails
                    .Where(i => i.lab_id == labId)
                    .FirstOrDefaultAsync();
                if (imagingDetail != null)
                {
                    _context.ImagingDetails.Remove(imagingDetail);
                }
            }
            else if (labTest.exam_type == "pulmonary")
            {
                var pulmonaryDetail = await _context.PulmonaryDetails
                    .Where(p => p.lab_id == labId)
                    .FirstOrDefaultAsync();
                if (pulmonaryDetail != null)
                {
                    _context.PulmonaryDetails.Remove(pulmonaryDetail);
                }
            }

            _context.LabTests.Remove(labTest);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Lab test deleted successfully" });
        }
        #endregion

        #region Diagnoses CRUD
        // 获取患者诊断
        [HttpGet("diagnosesInfo")]
        public async Task<IActionResult> GetDiagnosesByPatient(string patientId)
        {
            var diagnoses = await _context.Diagnoses
                .Where(d => d.patient_id == patientId)
                .ToListAsync();

            return Ok(new { success = true, data = diagnoses });
        }

        // 添加诊断
        [HttpPost("diagnosesAdd")]
        public async Task<IActionResult> AddDiagnosis([FromBody] DiagnosisDto diagnosisDto)
        {
            var newDiagnosis = new Diagnosis
            {
                diagnosis_id = Guid.NewGuid().ToString(),
                patient_id = diagnosisDto.PatientId,
                disease_name = diagnosisDto.DiseaseName,
                severity = diagnosisDto.Severity,
                description = diagnosisDto.Description
            };

            _context.Diagnoses.Add(newDiagnosis);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Diagnosis added successfully", data = newDiagnosis });
        }

        // 更新诊断
        [HttpPut("diagnosesUpd")]
        public async Task<IActionResult> UpdateDiagnosis(string diagnosisId, [FromBody] DiagnosisDto diagnosisDto)
        {
            var diagnosis = await _context.Diagnoses.FindAsync(diagnosisId);
            if (diagnosis == null)
            {
                return NotFound(new { success = false, message = "Diagnosis not found" });
            }

            diagnosis.disease_name = diagnosisDto.DiseaseName;
            diagnosis.severity = diagnosisDto.Severity;
            diagnosis.description = diagnosisDto.Description;

            _context.Diagnoses.Update(diagnosis);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Diagnosis updated successfully", data = diagnosis });
        }

        // 删除诊断
        [HttpDelete("diagnosesDel")]
        public async Task<IActionResult> DeleteDiagnosis(string diagnosisId)
        {
            var diagnosis = await _context.Diagnoses.FindAsync(diagnosisId);
            if (diagnosis == null)
            {
                return NotFound(new { success = false, message = "Diagnosis not found" });
            }

            _context.Diagnoses.Remove(diagnosis);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Diagnosis deleted successfully" });
        }
        #endregion
    }

    #region DTO Classes
    public class InsuranceDto
    {
        public string PatientId { get; set; }
        public string InsuranceType { get; set; }
    }

    public class ContactDto
    {
        public string PatientId { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
    }

    public class MedicalHistoryDto
    {
        public string AllergyHistory { get; set; }
    }

    public class FamilyHistoryDto
    {
        public string AllergyHistory { get; set; }
    }

    public class LabTestDto
    {
        public string PatientId { get; set; }
        public string ItemName { get; set; }
        public string ExamValue { get; set; }
        public string ExamType { get; set; }
        public string ExamDetails { get; set; }
    }

    public class DiagnosisDto
    {
        public string PatientId { get; set; }
        public string DiseaseName { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
    }
    #endregion
}