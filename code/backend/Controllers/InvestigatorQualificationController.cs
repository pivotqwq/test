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
    public class InvestigatorQualificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InvestigatorQualificationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/InvestigatorQualification
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvestigatorQualification>>> GetInvestigatorQualifications()
        {
            return await _context.InvestigatorQualifications.ToListAsync();
        }

        // POST: api/InvestigatorQualification/seed-default
        [HttpPost("seed-default")]
        public async Task<ActionResult<object>> SeedDefaultInvestigators()
        {
            try
            {
                // 检查是否已经有调查员记录
                var existingCount = await _context.InvestigatorQualifications.CountAsync();
                if (existingCount > 0)
                {
                    return Ok(new { 
                        message = $"数据库中已有 {existingCount} 个调查员记录，无需创建默认数据",
                        existing_count = existingCount
                    });
                }

                // 创建默认调查员记录
                var defaultInvestigators = new List<InvestigatorQualification>
                {
                    new InvestigatorQualification
                    {
                        investigator_id = "INV001",
                        name = "系统默认调查员",
                        qualification = "医学研究员",
                        institution = "第一人民医院",
                        position = "主任医师",
                        contact_phone = "138****0001"
                    },
                    new InvestigatorQualification
                    {
                        investigator_id = "INV002", 
                        name = "张调研员",
                        qualification = "临床医生",
                        institution = "第一人民医院",
                        position = "副主任医师",
                        contact_phone = "138****0002"
                    },
                    new InvestigatorQualification
                    {
                        investigator_id = "INV003",
                        name = "李调研员", 
                        qualification = "护理师",
                        institution = "第一人民医院",
                        position = "主管护师",
                        contact_phone = "138****0003"
                    }
                };

                _context.InvestigatorQualifications.AddRange(defaultInvestigators);
                await _context.SaveChangesAsync();

                return Ok(new {
                    message = "成功创建默认调查员记录",
                    created_count = defaultInvestigators.Count,
                    investigators = defaultInvestigators.Select(i => new { 
                        i.investigator_id, 
                        i.name, 
                        i.qualification 
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "创建默认调查员记录失败", 
                    error = ex.Message 
                });
            }
        }

        // GET: api/InvestigatorQualification/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetInvestigatorQualificationsPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.InvestigatorQualifications.CountAsync();
            var investigators = await _context.InvestigatorQualifications
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = investigators,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/InvestigatorQualification/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InvestigatorQualification>> GetInvestigatorQualification(string id)
        {
            var investigatorQualification = await _context.InvestigatorQualifications.FindAsync(id);

            if (investigatorQualification == null)
            {
                return NotFound();
            }

            return investigatorQualification;
        }

        // PUT: api/InvestigatorQualification/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvestigatorQualification(string id, InvestigatorQualification investigatorQualification)
        {
            if (id != investigatorQualification.investigator_id)
            {
                return BadRequest();
            }

            _context.Entry(investigatorQualification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InvestigatorQualificationExists(id))
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

        // POST: api/InvestigatorQualification
        [HttpPost]
        public async Task<ActionResult<InvestigatorQualification>> PostInvestigatorQualification(InvestigatorQualification investigatorQualification)
        {
            _context.InvestigatorQualifications.Add(investigatorQualification);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (InvestigatorQualificationExists(investigatorQualification.investigator_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetInvestigatorQualification", new { id = investigatorQualification.investigator_id }, investigatorQualification);
        }

        // DELETE: api/InvestigatorQualification/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvestigatorQualification(string id)
        {
            var investigatorQualification = await _context.InvestigatorQualifications.FindAsync(id);
            if (investigatorQualification == null)
            {
                return NotFound();
            }

            _context.InvestigatorQualifications.Remove(investigatorQualification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvestigatorQualificationExists(string id)
        {
            return _context.InvestigatorQualifications.Any(e => e.investigator_id == id);
        }
    }
}