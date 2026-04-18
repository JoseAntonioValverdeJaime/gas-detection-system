using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasApi.Models
{
    [Table("Alerts")]
    public class Alert
    {
        public int Id { get; set; }


        public string MeterId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Observation { get; set; }
        public string? Codigo { get; set; }
        public int? TechnicianId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? ReassignedAt { get; set; }
        public DateTime? AttendedAt { get; set; }

        public decimal? RiskLevel { get; set; }
        public string? AlertLevel { get; set; }
        public string? RejectReason { get; set; }

        // Navegación
        public Technician? Technician { get; set; }
    }
}
