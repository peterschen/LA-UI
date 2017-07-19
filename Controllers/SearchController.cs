using System;
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

        public IActionResult Index()
        {
            return View(new SearchViewModel());
        }

        public IActionResult Search(SearchViewModel model)
        {
            // Perform search
            var searcher = new LogSearcher(
                _settings.TenantId,
                _settings.ApplicationId,
                _settings.ApplicationKey, 
                _settings.SubscriptionId,
                _settings.ResourceGroup,
                _settings.WorkspaceName);

            model.Transactions = searcher.SearchTransactions(model.TransactionId, model.VehicleId);
            
            return View("Index", model);
        }
    }
}
