using System.Diagnostics;
using Cuahangchay.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cuahangchay.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult About() => View();

        public IActionResult Menu()
        {

            return View();
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
