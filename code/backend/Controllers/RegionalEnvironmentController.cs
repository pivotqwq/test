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
    public class RegionalEnvironmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegionalEnvironmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/RegionalEnvironment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegionalEnvironment>>> GetRegionalEnvironments()
        {
            return await _context.RegionalEnvironments.ToListAsync();
        }

        // GET: api/RegionalEnvironment/paged?page=1&limit=10
        [HttpGet("paged")]
        public async Task<ActionResult<object>> GetRegionalEnvironmentsPaged(int page = 1, int limit = 10)
        {
            var totalCount = await _context.RegionalEnvironments.CountAsync();
            var environments = await _context.RegionalEnvironments
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return Ok(new
            {
                code = 200,
                data = environments,
                total = totalCount,
                page = page,
                limit = limit,
                totalPages = (int)Math.Ceiling((double)totalCount / limit)
            });
        }

        // GET: api/RegionalEnvironment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegionalEnvironment>> GetRegionalEnvironment(string id)
        {
            var regionalEnvironment = await _context.RegionalEnvironments.FindAsync(id);

            if (regionalEnvironment == null)
            {
                return NotFound();
            }

            return regionalEnvironment;
        }

        // GET: api/RegionalEnvironment/default
        [HttpGet("default")]
        public async Task<ActionResult<RegionalEnvironment>> GetDefaultRegionalEnvironment()
        {
            // 检查是否有默认区域数据
            var defaultRegion = await _context.RegionalEnvironments.FirstOrDefaultAsync();
            
            if (defaultRegion == null)
            {
                // 创建默认区域数据
                defaultRegion = new RegionalEnvironment
                {
                    region_id = "DEFAULT_001",
                    region_name = "默认区域",
                    green_space_rate = 30.0m,
                    air_quality_index = 85,
                    pollen_concentration = "中",
                    climate_type = "温带",
                    avg_temperature = 20.0m,
                    humidity_level = 60.0m,
                    update_date = DateTime.UtcNow
                };
                
                _context.RegionalEnvironments.Add(defaultRegion);
                await _context.SaveChangesAsync();
            }
            
            return defaultRegion;
        }

        // POST: api/RegionalEnvironment/seed
        [HttpPost("seed")]
        public async Task<ActionResult<object>> SeedRegionalEnvironmentData()
        {
            try
            {
                // 检查是否已有数据
                var existingCount = await _context.RegionalEnvironments.CountAsync();
                if (existingCount > 0)
                {
                    return Ok(new { message = "区域环境数据已存在", count = existingCount });
                }

                // 创建示例区域环境数据
                var regions = new List<RegionalEnvironment>
                {
                    new RegionalEnvironment
                    {
                        region_id = "REGION_001",
                        region_name = "市中心区域",
                        green_space_rate = 25.5m,
                        air_quality_index = 95,
                        pollen_concentration = "高",
                        climate_type = "温带",
                        avg_temperature = 22.0m,
                        humidity_level = 65.0m,
                        update_date = DateTime.UtcNow
                    },
                    new RegionalEnvironment
                    {
                        region_id = "REGION_002",
                        region_name = "郊区区域",
                        green_space_rate = 45.0m,
                        air_quality_index = 65,
                        pollen_concentration = "中",
                        climate_type = "温带",
                        avg_temperature = 20.0m,
                        humidity_level = 58.0m,
                        update_date = DateTime.UtcNow
                    },
                    new RegionalEnvironment
                    {
                        region_id = "REGION_003",
                        region_name = "工业区域",
                        green_space_rate = 15.0m,
                        air_quality_index = 120,
                        pollen_concentration = "低",
                        climate_type = "温带",
                        avg_temperature = 23.0m,
                        humidity_level = 70.0m,
                        update_date = DateTime.UtcNow
                    }
                };

                _context.RegionalEnvironments.AddRange(regions);
                await _context.SaveChangesAsync();

                return Ok(new { message = "区域环境数据初始化成功", count = regions.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"初始化区域环境数据失败: {ex.Message}" });
            }
        }

        // Note: RegionalEnvironment doesn't have patient_id as it's region-level data
        // If needed, implement patient-region lookup through address or other means

        // PUT: api/RegionalEnvironment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegionalEnvironment(string id, RegionalEnvironment regionalEnvironment)
        {
            if (id != regionalEnvironment.region_id)
            {
                return BadRequest();
            }

            _context.Entry(regionalEnvironment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegionalEnvironmentExists(id))
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

        // POST: api/RegionalEnvironment
        [HttpPost]
        public async Task<ActionResult<RegionalEnvironment>> PostRegionalEnvironment(RegionalEnvironment regionalEnvironment)
        {
            _context.RegionalEnvironments.Add(regionalEnvironment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RegionalEnvironmentExists(regionalEnvironment.region_id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRegionalEnvironment", new { id = regionalEnvironment.region_id }, regionalEnvironment);
        }

        // DELETE: api/RegionalEnvironment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRegionalEnvironment(string id)
        {
            var regionalEnvironment = await _context.RegionalEnvironments.FindAsync(id);
            if (regionalEnvironment == null)
            {
                return NotFound();
            }

            _context.RegionalEnvironments.Remove(regionalEnvironment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RegionalEnvironmentExists(string id)
        {
            return _context.RegionalEnvironments.Any(e => e.region_id == id);
        }
    }
}