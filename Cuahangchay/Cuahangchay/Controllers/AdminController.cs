﻿using Microsoft.AspNetCore.Mvc;

namespace Cuahangchay.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}