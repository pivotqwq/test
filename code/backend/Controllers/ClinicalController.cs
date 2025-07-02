using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using static backend.Data.ApplicationDbContext;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseService _databaseService;

        public ClinicalController(ApplicationDbContext context, DatabaseService databaseService)
        {
            _context = context;
            _databaseService = databaseService;
        }

        [HttpGet("patient")]
        public async Task<IActionResult> GetPatients(string? name, int page = 1,int limit = 10)
        {
            var query = _context.PatientBasicInfos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.name.Contains(name));
            }

            // 获取总数
            int totalCount = await query.CountAsync();

            // 分页查询
            var patients = await query
            .OrderBy(p => p.name) // 默认按名字排序
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var patientDtos = patients.Select(p => new PatientDto
            {
                PatientId = p.patient_id,
                MedicalRecordNo = p.patient_id, // PatientBasicInfo没有medical_record_no，使用patient_id
                Name = p.name,
                Gender = p.gender,
                BirthDate = p.birth_date.ToString("yyyy-MM-dd"),
                Address = p.residence_type
            }).ToList();

            return Ok(new { code = 200,tot=totalCount, rows = patientDtos });
        }

        [HttpPost("patientAdd")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
        {
            var existingPatient = await _context.PatientBasicInfos
                .FirstOrDefaultAsync(p => p.patient_id == request.MedicalRecordNo);

            if (existingPatient != null)
            {
                return BadRequest(new { code = 400, message = "患者ID已存在" });
            }

            var newPatient = new PatientBasicInfo
            {
                patient_id = request.MedicalRecordNo ?? Guid.NewGuid().ToString(),
                name = request.Name ?? "未知",
                gender = request.Gender ?? "M",
                birth_date = DateTime.TryParse(request.BirthDate, out var birthDate) ? birthDate : DateTime.UtcNow,
                residence_type = request.Address ?? "未知",
                allergy_history = "待补充",
                create_time = DateTime.UtcNow
            };

            _context.PatientBasicInfos.Add(newPatient);
            await _context.SaveChangesAsync();

            return Ok(new { code = 200, data = newPatient });
            
        }

        [HttpPut("patientUpd")]
        public async Task<IActionResult> UpdatePatient(string patientId,[FromBody] UpdatePatientRequest request)
        {
            var patient = await _context.PatientBasicInfos.FindAsync(patientId);
            if (patient == null)
            {
                return NotFound(new { code = 404, message = "患者不存在" });
            }

            // 检查患者ID是否被其他患者使用
            if (!string.IsNullOrEmpty(request.MedicalRecordNo))
            {
                var existingWithMr = await _context.PatientBasicInfos
                    .AnyAsync(p => p.patient_id == request.MedicalRecordNo && p.patient_id != patientId);
                if (existingWithMr)
                {
                    return BadRequest(new { code = 400, message = "患者ID已被其他患者使用" });
                }
            }

            // 更新字段（只更新非空字段）
            if (!string.IsNullOrEmpty(request.MedicalRecordNo))
                patient.patient_id = request.MedicalRecordNo;
            if (!string.IsNullOrEmpty(request.Name))
                patient.name = request.Name;
            if (!string.IsNullOrEmpty(request.Gender))
                patient.gender = request.Gender;
            if (!string.IsNullOrEmpty(request.BirthDate))
            {
                if (DateTime.TryParse(request.BirthDate, out var birthDate))
                    patient.birth_date = birthDate;
            }
            if (!string.IsNullOrEmpty(request.Address))
                patient.residence_type = request.Address;

            patient.update_time = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { code = 200, message = "更新成功" });
        }

        [HttpDelete("patientDel")]
        public async Task<IActionResult> DeletePatient(string patientId)
        {
            var patient = await _context.PatientBasicInfos.FindAsync(patientId);
            if (patient == null)
            {
                return NotFound(new { code = 404, message = "患者不存在" });
            }

            _context.PatientBasicInfos.Remove(patient);

            await _context.SaveChangesAsync();

            return Ok(new { code = 204, message = "删除成功" });
        }

        public class UpdatePatientRequest
        {
            public string? MedicalRecordNo { get; set; }
            public string? Name { get; set; }
            public string? Gender { get; set; }
            public string? BirthDate { get; set; }
            public string? Address { get; set; }
        }

        public class CreatePatientRequest
        {
            public string? MedicalRecordNo { get; set; }
            public string? Name { get; set; }
            public string? Gender { get; set; }
            public string? BirthDate { get; set; }
            public string? Address { get; set; }
        }

        public class PatientDto
        {
            public string? PatientId { get; set; }
            public string? MedicalRecordNo { get; set; }
            public string? Name { get; set; }
            public string? Gender { get; set; }
            public string? BirthDate { get; set; }
            public string? Address { get; set; }
        }

    }
}
