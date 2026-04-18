using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasApi.Models
{
    [Table("AlertAudit")]
    public class AlertAudit
    {
        public int Id { get; set; }

        public int AlertId { get; set; }
        public Alert Alert { get; set; } = null!;

        public string ChangeType { get; set; } = null!; // EDICION, ASIGNACION...
        public DateTime ChangeDate { get; set; }
        public string UserName { get; set; } = "sistema";
        public string? Detail { get; set; }
    }
}
