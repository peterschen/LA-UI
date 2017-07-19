using System;
using Microsoft.AspNetCore.Mvc;

namespace laui.Controllers
{
    public class HomeController : Controller
    {
        private readonly Settings _settings;

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
