using System.Diagnostics;
using Cuahangchay.Data;
using Cuahangchay.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cuahangchay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Menu()
        {
            List<MonChay> monChaylist = _context.MonChays.ToList();
            return View(monChaylist);
        }

        [HttpGet]
        public IActionResult Reservation() => View();

        [HttpPost]
        public IActionResult Reservation(KhachHang res)
        {
            if (ModelState.IsValid)
                return RedirectToAction("Success");
            return View(res);
        }

        public IActionResult Contact() => View();

        public IActionResult Success() => View();
    }

}

