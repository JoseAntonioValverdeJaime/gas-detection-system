using GasApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   // → api/Alerts
    public class AlertsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AlertsController(AppDbContext db)
        {
            _db = db;
        }

        // =======================
        // CRUD BÁSICO (ya lo tenías)
        // =======================

        // GET: api/Alerts
        [HttpGet]
        public IActionResult GetAlerts()
        {
            var alerts = _db.Alerts
                .Include(a => a.Technician)
                .ToList();

            return Ok(alerts);
        }

        // GET: api/Alerts/{id}
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

        // POST: api/Alerts
        [HttpPost]
        public IActionResult CreateAlert([FromBody] Alert alert)
        {
            
            if (alert.CreatedAt == default)
                alert.CreatedAt = DateTime.UtcNow;

            _db.Alerts.Add(alert);
            _db.SaveChanges();

            return Ok(new { message = "Alerta creada", alert });
        }

        // PUT: api/Alerts/{id}
      
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

        // DELETE: api/Alerts/{id}
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

        // =======================
        // ENDPOINTS DEL SPRINT 2
        // =======================

        // POST: api/Alerts/{id}/assign   → HU005 Asignar técnico
        [HttpPost("{id}/assign")]
        public IActionResult AssignTechnician(int id, [FromBody] int technicianId)
        {
            var alert = _db.Alerts.FirstOrDefault(a => a.Id == id);
            if (alert == null)
                return NotFound(new { message = "No existe la alerta" });

            var tech = _db.Technicians.FirstOrDefault(t => t.Id == technicianId && t.IsActive);
            if (tech == null)
                return BadRequest(new { message = "Técnico no válido o inactivo" });

            alert.TechnicianId = technicianId;
            alert.AssignedAt = DateTime.UtcNow;
            alert.Status = "Asignada";

            RegisterAudit(alert.Id, "ASIGNACION", $"Asignada al técnico {tech.FullName}");

            _db.SaveChanges();

            return Ok(alert);
        }

        // PUT: api/Alerts/{id}/reassign  → HU006 Reasignar técnico
        [HttpPut("{id}/reassign")]
        public IActionResult ReassignTechnician(int id, [FromBody] int newTechnicianId)
        {
            var alert = _db.Alerts.FirstOrDefault(a => a.Id == id);
            if (alert == null)
                return NotFound(new { message = "No existe la alerta" });

            var tech = _db.Technicians.FirstOrDefault(t => t.Id == newTechnicianId && t.IsActive);
            if (tech == null)
                return BadRequest(new { message = "Nuevo técnico no válido o inactivo" });

            alert.TechnicianId = newTechnicianId;
            alert.ReassignedAt = DateTime.UtcNow;
            alert.Status = "Asignada";

            RegisterAudit(alert.Id, "REASIGNACION", $"Reasignada al técnico {tech.FullName}");

            _db.SaveChanges();

            return Ok(alert);
        }

        // PUT: api/Alerts/{id}/confirm   → HU007 Confirmar atención
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

        // =======================
        // MÉTODO PRIVADO AUDITORÍA
        // =======================
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
