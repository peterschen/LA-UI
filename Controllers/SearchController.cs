using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using laui;
using laui.Models;

namespace laui.Controllers
{
    public class SearchController : Controller
    {
        private readonly Settings _settings;

        public SearchController(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
