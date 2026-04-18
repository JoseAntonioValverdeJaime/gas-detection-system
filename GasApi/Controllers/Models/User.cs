using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasApi.Models
{
    [Table("Users")]
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
    }
}