using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using laui;
using laui.ViewModels;

namespace laui.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly Settings _settings;

        public ConfigurationController(IOptions<Settings> settings)
        {
            _settings = settings.Value;
        }

        public IActionResult Index()
        {
            var model = InitializeModel();
            return View(model);
        }

        public IActionResult Test()
        {
            var model = InitializeModel(true);

            // Test HTTP Data Collector API
            var collector = new LogCollector(_settings.WorkspaceId, _settings.WorkspaceKey);
            var collectorConfigurationValid = collector.WriteTestData();

            // Test Log Search API
            var searcher = new LogSearcher(_settings.TenantId, _settings.ApplicationId, _settings.ApplicationKey, 
                _settings.SubscriptionId, _settings.ResourceGroup, _settings.WorkspaceName);
            var searcherConfigurationValid = searcher.ReadTestData();
            
            model.IsValid = collectorConfigurationValid && searcherConfigurationValid;
            return View("Index", model);
        }

        private ConfigurationViewModel InitializeModel(bool isTested = false)
        {
            var model = new ConfigurationViewModel {
                TenantId = _settings.TenantId,
                SubscriptionId = _settings.SubscriptionId,
                ApplicationId = _settings.ApplicationId, 
                ApplicationKey = _settings.ApplicationKey,
                ResourceGroup = _settings.ResourceGroup,
                WorkspaceId = _settings.WorkspaceId,
                WorkspaceKey = _settings.WorkspaceKey,
                WorkspaceName = _settings.WorkspaceName };

            if(isTested)
            {
                model.IsTested = isTested;
            }

            return model;
        }
    }
}
