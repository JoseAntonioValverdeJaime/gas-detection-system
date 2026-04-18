using GasApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult CreateUser(User user)
        {
            user.CreatedAt = DateTime.Now;
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        }
    }
}