using GasApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;



namespace GasApi.Controllers
{   

public class AssignRequest
{
    public int TechnicianId { get; set; }
    public DateTime FechaVisita { get; set; }
}

    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AlertsController(AppDbContext db)
        {
            _db = db;
        }

[HttpGet]
public IActionResult GetAlerts()
{
    var alerts = _db.Alerts
        .Include(a => a.Technician)
        .Select(a => new {
    a.Id,
    a.MeterId,
    a.Status,
    a.Observation,
    a.Codigo,
    a.TechnicianId,
    a.CreatedAt,
    a.AssignedAt,
    a.ReassignedAt,
    a.AttendedAt,
    a.RiskLevel,
    a.AlertLevel,
    a.RejectReason
})
        .ToList();

    return Ok(alerts);
}

        [HttpGet("{id}")]
        public IActionResult GetAlertById(int id)
        {
            var alert = _db.Alerts
                .Include(a => a.Technician)
                .FirstOrDefault(a => a.Id == id);

            if (alert == null)
                return NotFound(new { message = "No existe la alerta" });

            return Ok(alert);
        }

        [HttpPost]
        public IActionResult CreateAlert([FromBody] Alert alert)
        {
            if (alert.CreatedAt == default)
                alert.CreatedAt = DateTime.UtcNow;

            _db.Alerts.Add(alert);
            _db.SaveChanges();

            return Ok(new { message = "Alerta creada", alert });
        }

        [HttpPut("{id}")]
        public IActionResult EditAlert(int id, [FromBody] Alert edited)
        {
            var alert = _db.Alerts.FirstOrDefault(a => a.Id == id);

            if (alert == null)
                return NotFound(new { message = "No existe la alerta" });

            alert.MeterId = edited.MeterId;
            alert.Status = edited.Status;
            alert.Observation = edited.Observation;
            alert.Codigo = edited.Codigo;
            alert.TechnicianId = edited.TechnicianId;
            alert.CreatedAt = edited.CreatedAt;
            alert.AssignedAt = edited.AssignedAt;
            alert.ReassignedAt = edited.ReassignedAt;
            alert.AttendedAt = edited.AttendedAt;
            alert.RiskLevel = edited.RiskLevel;
            alert.AlertLevel = edited.AlertLevel;
            alert.RejectReason = edited.RejectReason;

            _db.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAlert(int id)
        {
            var alert = _db.Alerts.FirstOrDefault(a => a.Id == id);
            if (alert == null)
                return NotFound(new { message = "No existe la alerta" });

            _db.Alerts.Remove(alert);
            _db.SaveChanges();

            return Ok(new { message = "Alerta eliminada" });
        }

       [HttpPost("{id}/assign")]
public IActionResult AssignTechnician(int id, [FromBody] AssignRequest request)
{
    var alert = _db.Alerts.FirstOrDefault(a => a.Id == id);
    if (alert == null)
        return NotFound(new { message = "No existe la alerta" });

    var newTech = _db.Technicians.FirstOrDefault(t => t.Id == request.TechnicianId && t.IsActive);
    if (newTech == null)
        return BadRequest(new { message = "Técnico no válido o inactivo" });

    // Si ya tenía técnico → reasignación
    if (alert.TechnicianId.HasValue)
    {
        if (alert.Status == "Asignada")
            alert.Status = "Reasignada";

        var previousTech = _db.Technicians.FirstOrDefault(t => t.Id == alert.TechnicianId.Value);
        if (previousTech != null && previousTech.TotalAssignments > 0)
            previousTech.TotalAssignments -= 1;
    }

    alert.TechnicianId = request.TechnicianId;

    // 🔥 AQUÍ ESTÁ LA SOLUCIÓN
    alert.AssignedAt = request.FechaVisita;

    if (alert.Status == null || alert.Status == "Validada")
        alert.Status = "Asignada";

    newTech.TotalAssignments += 1;

    _db.SaveChanges();

    return Ok(new { message = "Asignación guardada correctamente" });
}

        [HttpPut("{id}/reassign")]
        public IActionResult ReassignTechnician(int id, [FromBody] int newTechnicianId)
        {
            var alert = _db.Alerts.FirstOrDefault(a => a.Id == id);
            if (alert == null)
                return NotFound(new { message = "No existe la alerta" });

            var newTech = _db.Technicians.FirstOrDefault(t => t.Id == newTechnicianId && t.IsActive);
            if (newTech == null)
                return BadRequest(new { message = "Nuevo técnico no válido o inactivo" });

            if (alert.TechnicianId.HasValue)
            {
                var previousTech = _db.Technicians.FirstOrDefault(t => t.Id == alert.TechnicianId.Value);
                if (previousTech != null && previousTech.TotalAssignments > 0)
                    previousTech.TotalAssignments -= 1;
            }

            alert.TechnicianId = newTechnicianId;
            alert.ReassignedAt = DateTime.UtcNow;
            alert.Status = "Reasignada";

            newTech.TotalAssignments += 1;

            _db.SaveChanges();
            return Ok(new { message = "Reasignación guardada correctamente" });
        }

        [HttpPut("{id}/devolver")]
public IActionResult DevolverAValidada(int id)
{
    var alert = _db.Alerts.FirstOrDefault(a => a.Id == id);
    if (alert == null)
        return NotFound();

    // Quitar el técnico asignado y el estado
    alert.TechnicianId = null;
    alert.Status = "Validada";
    alert.AssignedAt = null;

    _db.SaveChanges();
    return Ok(new { message = "Alerta devuelta a Validada correctamente." });
}



        [HttpPut("{id}/close")]
public async Task<IActionResult> CloseAlert(int id, IFormFile archivo)
{
    var alert = _db.Alerts.FirstOrDefault(a => a.Id == id);
    if (alert == null)
        return NotFound(new { message = "No existe la alerta" });

    // Validar archivo
    if (archivo == null || archivo.Length == 0)
        return BadRequest(new { message = "Debe adjuntar un archivo" });

    // (Opcional) Aquí podrías guardar el archivo si quieres

    if (alert.TechnicianId.HasValue)
    {
        var tech = _db.Technicians.FirstOrDefault(t => t.Id == alert.TechnicianId.Value);
        if (tech != null && tech.TotalAssignments > 0)
            tech.TotalAssignments -= 1;
    }

    alert.TechnicianId = null;
    alert.Status = "Atendida";
    alert.AttendedAt = DateTime.UtcNow;

    _db.SaveChanges();

    return Ok(new { message = "Alerta cerrada correctamente" });
}

        [HttpPut("{id}/confirm")]
        public IActionResult ConfirmAttention(int id)
        {
            var alert = _db.Alerts.FirstOrDefault(a => a.Id == id);
            if (alert == null)
                return NotFound(new { message = "No existe la alerta" });

            alert.AttendedAt = DateTime.UtcNow;
            alert.Status = "Cerrada";

            RegisterAudit(alert.Id, "CONFIRMACION", "Atención de alerta confirmada");

            _db.SaveChanges();

            return Ok(alert);
        }

        private void RegisterAudit(int alertId, string type, string detail)
        {
            var audit = new AlertAudit
            {
                AlertId = alertId,
                ChangeType = type,
                Detail = detail,
                ChangeDate = DateTime.UtcNow,
                UserName = "sistema"
            };

            _db.AlertAudit.Add(audit);
        }
    }
}