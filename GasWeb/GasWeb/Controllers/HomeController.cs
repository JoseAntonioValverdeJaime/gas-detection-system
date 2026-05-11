using GasWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace GasWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TecnicosAdmin()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult AlertDetail(int id)
        {
            ViewBag.AlertId = id;
            return View();
        }

        public IActionResult Brigada()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin" && role != "Supervisor")
            {
                return RedirectToAction("Dashboard");
            }

            return View();
        }

        [HttpPost]
        public IActionResult GuardarSesion([FromBody] JsonElement data)
        {
            string role = data.GetProperty("role").GetString();

            HttpContext.Session.SetString("Role", role);

            return Ok();
        }

        public IActionResult AlertDetailBrigada(int id)
        {
            ViewBag.AlertId = id;
            return View();
        }

        public IActionResult TecnicosBusqueda()
        {
            return View();
        }

        public IActionResult TecnicoDetalle(int id)
        {
            ViewBag.Id = id; // Pasamos el ID a la vista
            return View();
        }


    }
}
