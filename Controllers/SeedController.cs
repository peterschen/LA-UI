using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using laui;
using laui.Models;

namespace laui.Controllers
{
    public class SeedController : Controller
    {
        private readonly Settings _settings;

        public SeedController(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new SeedModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Seed(SeedModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var recordString = RecordFactory.GenerateTraceRecords(
                model.VehicleId,
                model.TotalTransactions, 
                model.BadTransactions.HasValue ? model.BadTransactions.Value : 0);
            
            var collector = new LogCollector(_settings.WorkspaceId, _settings.WorkspaceKey);
            model.IsSuccess = await collector.Collect(Constants.RecordTypeTrace, recordString);
            model.IsSeeded = true;

            return View("Index", model);
        }
    }
}
