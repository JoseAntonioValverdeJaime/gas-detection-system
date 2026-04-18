using System.ComponentModel.DataAnnotations.Schema;

namespace GasApi.Models
{
    [Table("Technicians")]
    public class Technician
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        
    }
}
