using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using laui.ViewModels;

namespace laui.Controllers
{
    public class SearchController : Controller
    {
        private readonly Settings _settings;

        public SearchController(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        [HttpGet]
        public IActionResult Events()
        {
            return View(new SearchViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Events(SearchViewModel model)
        {
            // Perform search
            var searcher = new LogSearcher(
                _settings.TenantId,
                _settings.ApplicationId,
                _settings.ApplicationKey, 
                _settings.SubscriptionId,
                _settings.ResourceGroup,
                _settings.WorkspaceId,
                _settings.WorkspaceName);

            model.Records = await searcher.SearchTraceRecords(model.TransactionId, model.VehicleId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Transactions()
        {
            return View(new SearchViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Transactions(SearchViewModel model)
        {
            // Perform search
            var searcher = new LogSearcher(
                _settings.TenantId,
                _settings.ApplicationId,
                _settings.ApplicationKey, 
                _settings.SubscriptionId,
                _settings.ResourceGroup,
                _settings.WorkspaceId,
                _settings.WorkspaceName);

            model.Transactions = await searcher.SearchTransactions(model.TransactionId, model.VehicleId);
            return View(model);
        }
    }
}
