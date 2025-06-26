using backend.Data;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class patientInfoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseService _databaseService;

        public patientInfoController(ApplicationDbContext context, DatabaseService databaseService)
        {
            _context = context;
            _databaseService = databaseService;
        }

        // GET: api/patientInfo/allPatients
        [HttpGet("allPatients")]
        public async Task<IActionResult> GetUserBy(short page = 1, short limit = 10)
        {
            var allusers = await _context.Patients.AsQueryable().ToListAsync();
            var users = await _context.Patients.AsQueryable().Skip((page - 1) * limit)
                    .Take(limit).ToListAsync();


            if (users == null)
            {
                return NotFound(new { code = 404, message = "用户未找到" });
            }

            int totCount = allusers.Count();

            return Ok(new
            {
                code = 200,
                tot = totCount,
                data = users,
            });
        }

        // GET: api/patientInfo/detail
        [HttpGet("detail")]
        public async Task<IActionResult> GetPatientDetail(string medicalRecordNo)
        {
            try
            {
                // 1. 根据病历号查询患者基本信息
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.medical_record_no == medicalRecordNo.Trim());

                if (patient == null)
                {
                    return NotFound(new { code = 404, message = "患者不存在" });
                }

                /*
                // 2. 查询关联信息
                var treatmentRecords = await _context.TreatmentRecords
                    .Where(r => r.PatientId == patient.Id)
                    .ToListAsync();

                var medicalReports = await _context.MedicalReports
                    .Where(r => r.PatientId == patient.Id)
                    .ToListAsync();

                var medicationHistory = await _context.Medications
                    .Where(m => m.PatientId == patient.Id)
                    .ToListAsync();
                
                // 3. 组装返回数据
                var result = new
                {
                    code = 200,
                    data = new
                    {
                        medical_record_no = patient.medical_record_no,
                        name = patient.name,
                        gender = patient.gender,
                        age = CalculateAge(patient.birth_date),
                        birth_date = patient.BirthDate.ToString("yyyy-MM-dd"),
                        phone = patient.Phone,
                        address = patient.Address,
                        status = patient.Status,
                        //treatment_records = treatmentRecords,
                        medical_reports = medicalReports,
                        medication_history = medicationHistory
                    }
                };*/

                return Ok(new {code=200,data=patient});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, message = "服务器内部错误" });
            }
        }

    }
}
