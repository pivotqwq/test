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
            var query = _context.Patients.AsQueryable();

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
                PatientId = p.id,
                MedicalRecordNo = p.medical_record_no,
                Name = p.name,
                Gender = p.gender,
                BirthDate = p.birth_date,
                Address = p.address
            }).ToList();

            return Ok(new { code = 200,tot=totalCount, rows = patientDtos });
        }

        [HttpPost("patientAdd")]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientRequest request)
        {
            var existingPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.medical_record_no == request.MedicalRecordNo);

            if (existingPatient != null)
            {
                return BadRequest(new { code = 400, message = "病历号已存在" });
            }

            var newPatient = new patients
            {
                id = Guid.NewGuid().ToString(),
                medical_record_no = request.MedicalRecordNo,
                name = request.Name,
                gender = request.Gender,
                birth_date = request.BirthDate,
                address = request.Address
            };

            _context.Patients.Add(newPatient);
            await _context.SaveChangesAsync();

            return Ok(new { code = 200, data = newPatient });
            
        }

        [HttpPut("patientUpd")]
        public async Task<IActionResult> UpdatePatient(string patientId,[FromBody] UpdatePatientRequest request)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
            {
                return NotFound(new { code = 404, message = "患者不存在" });
            }

            // 检查病历号是否被其他患者使用
            if (!string.IsNullOrEmpty(request.MedicalRecordNo))
            {
                var existingWithMr = await _context.Patients
                    .AnyAsync(p => p.medical_record_no == request.MedicalRecordNo && p.id != patientId);
                if (existingWithMr)
                {
                    return BadRequest(new { code = 400, message = "病历号已被其他患者使用" });
                }
            }

            // 更新字段（只更新非空字段）
            if (!string.IsNullOrEmpty(request.MedicalRecordNo))
                patient.medical_record_no = request.MedicalRecordNo;
            if (!string.IsNullOrEmpty(request.Name))
                patient.name = request.Name;
            if (!string.IsNullOrEmpty(request.Gender))
                patient.gender = request.Gender;
            if (!string.IsNullOrEmpty(request.BirthDate))
                patient.birth_date = request.BirthDate;
            if (!string.IsNullOrEmpty(request.Address))
                patient.address = request.Address;

            await _context.SaveChangesAsync();

            return Ok(new { code = 200, message = "更新成功" });
        }

        [HttpDelete("patientDel")]
        public async Task<IActionResult> DeletePatient(string patientId)
        {
            var patient = await _context.Patients.FindAsync(patientId);
            if (patient == null)
            {
                return NotFound(new { code = 404, message = "患者不存在" });
            }

            _context.Patients.Remove(patient);

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
