using GasApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TechniciansController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TechniciansController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Technicians
        [HttpGet]
public IActionResult GetTechnicians()
{   
    var technicians = _db.Technicians
        .Select(t => new
        {
            t.Id,
            t.FullName,
            t.Dni,
            t.Phone,
            t.CoverageZone,
            t.IsActive,
            t.TotalAssignments
        })
        .ToList();

    return Ok(technicians);
}




[HttpPost("rebuild-technician-counts")]
public IActionResult RebuildTechnicianCounts()
{
    var technicians = _db.Technicians.ToList();

    foreach (var tech in technicians)
        tech.TotalAssignments = 0;

    var counts = _db.Alerts
        .Where(a => a.TechnicianId.HasValue)
        .GroupBy(a => a.TechnicianId!.Value)
        .Select(g => new
        {
            TechnicianId = g.Key,
            Count = g.Count()
        })
        .ToList();

    foreach (var item in counts)
    {
        var tech = technicians.FirstOrDefault(t => t.Id == item.TechnicianId);
        if (tech != null)
            tech.TotalAssignments = item.Count;
    }

    _db.SaveChanges();
    return Ok(new { message = "Contadores recalculados correctamente" });
}

[HttpGet("{id}/alerts")]
public IActionResult GetTechnicianAlerts(int id) 
{
    var alertas = _db.Alerts
        .Where(a => a.TechnicianId == id && (a.Status == "Asignada" || a.Status == "Reasignada"))

            .Select(a => new {
            Codigo = a.Codigo ?? "-",
            Status = a.Status,
            Nivel = a.AlertLevel ?? "-",
            Medidor = a.MeterId,
            FechaCreacion = a.CreatedAt,
            Observacion = a.Observation ?? "-"
        })

        .ToList();
    return Ok(alertas);
}

[HttpGet("available")]
public IActionResult GetAvailableTechnicians()
{
    var tecnicosConCarga = _db.Technicians
    .Where(t => t.IsActive) // Esto ya filtra, pero ahora envíalo al front
    .Select(t => new {
        t.Id,
        t.FullName,
        t.CoverageZone,
        t.Dni,
        t.Phone,
        t.IsActive, // <--- AGREGA ESTA LÍNEA
        CargaActual = _db.Alerts.Count(a => a.TechnicianId == t.Id && a.Status == "Asignada") 
    })
    .ToList();

    return Ok(tecnicosConCarga);
}

[HttpPut("{id}/estado")]
public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoRequest body)
{
    var tecnico = await _db.Technicians.FindAsync(id);
    if (tecnico == null) return NotFound();

    tecnico.IsActive = body.IsActive;

    await _db.SaveChangesAsync();

    return Ok();
}

[HttpPut("{id}/toggle")]
public IActionResult ToggleTecnico(int id)
{
    var tecnico = _db.Technicians.FirstOrDefault(t => t.Id == id);

    if (tecnico == null)
        return NotFound(new { message = "Técnico no encontrado" });

    // Alternar estado (true/false)
    tecnico.IsActive = !tecnico.IsActive;

    _db.SaveChanges();

    return Ok(new 
    { 
        message = "Estado actualizado", 
        tecnico.Id, 
        tecnico.IsActive 
    });
}


        // GET: api/Technicians/{id}
        [HttpGet("{id}")]
        public IActionResult GetTechnicianById(int id)
        {
            var technician = _db.Technicians
                .FirstOrDefault(t => t.Id == id);

            if (technician == null)
                return NotFound(new { message = "No existe el técnico" });

            return Ok(technician);
        }
    }
}