using System.ComponentModel.DataAnnotations.Schema;

namespace GasApi.Models
{
    [Table("Technicians")]
    public class Technician
    {
        public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Dni { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int TotalAssignments { get; set; }
    public int TotalAttended { get; set; }
    public string? CoverageZone { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    
    
    }
}