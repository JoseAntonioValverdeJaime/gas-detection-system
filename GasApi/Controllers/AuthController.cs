using GasApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == request.Email &&
                u.Password == request.Password &&
                u.IsActive);

            if (user == null)
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            return Ok(new
            {
                message = "Login correcto",
                user.Id,
                user.FullName,
                user.Email,
                user.Role
            });
        }
    }
}