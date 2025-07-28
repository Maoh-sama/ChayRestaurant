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
            var dishes = new List<Dish>
        {
            new Dish { Id = 1, Name = "Phở Bò", Price = 80000, ImageUrl = "/images/pho.jpg" },
            new Dish { Id = 2, Name = "Bánh Mì", Price = 40000, ImageUrl = "/images/banhmi.jpg" }
        };
            return View(dishes);
        }

        [HttpGet]
        public IActionResult Reservation() => View();

        [HttpPost]
        public IActionResult Reservation(Reservation res)
        {
            if (ModelState.IsValid)
                return RedirectToAction("Success");
            return View(res);
        }

        public IActionResult Contact() => View();

        public IActionResult Success() => View();
    }

}
