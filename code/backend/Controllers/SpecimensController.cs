using backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class SpecimensController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SpecimensController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Specimens
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpecimenInfo>>> GetSpecimens()
    {
        return await _context.SpecimenInfos.ToListAsync();
    }

    // GET: api/Specimens/paged?page=1&limit=10
    [HttpGet("paged")]
    public async Task<ActionResult<object>> GetSpecimensPaged(int page = 1, int limit = 10)
    {
        var totalCount = await _context.SpecimenInfos.CountAsync();
        var specimens = await _context.SpecimenInfos
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return Ok(new
        {
            code = 200,
            data = specimens,
            total = totalCount,
            page = page,
            limit = limit,
            totalPages = (int)Math.Ceiling((double)totalCount / limit)
        });
    }

    // GET: api/Specimens/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SpecimenInfo>> GetSpecimen(string id)
    {
        var specimen = await _context.SpecimenInfos.FindAsync(id);

        if (specimen == null)
        {
            return NotFound();
        }

        return specimen;
    }

    // GET: api/Specimens/patient/5
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<SpecimenInfo>>> GetSpecimensByPatient(string patientId)
    {
        return await _context.SpecimenInfos
            .Where(s => s.patient_id == patientId)
            .ToListAsync();
    }

    // POST: api/Specimens
    [HttpPost]
    public async Task<ActionResult<SpecimenInfo>> PostSpecimen(SpecimenInfo specimen)
    {
        _context.SpecimenInfos.Add(specimen);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSpecimen", new { id = specimen.specimen_id }, specimen);
    }

    // PUT: api/Specimens/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSpecimen(string id, SpecimenInfo specimen)
    {
        if (id != specimen.specimen_id)
        {
            return BadRequest();
        }

        _context.Entry(specimen).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SpecimenExists(id))
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

    // DELETE: api/Specimens/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecimen(string id)
    {
        var specimen = await _context.SpecimenInfos.FindAsync(id);
        if (specimen == null)
        {
            return NotFound();
        }

        _context.SpecimenInfos.Remove(specimen);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SpecimenExists(string id)
    {
        return _context.SpecimenInfos.Any(e => e.specimen_id == id);
    }
}