using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using laui;
using laui.Models;

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
                _settings.SubscriptionId, _settings.WorkspaceId);
            var searcherConfigurationValid = searcher.ReadTestData();
            
            model.IsValid = collectorConfigurationValid && searcherConfigurationValid;
            return View("Index", model);
        }

        private ConfigurationModel InitializeModel(bool isTested = false)
        {
            var model = new ConfigurationModel {
                TenantId = _settings.TenantId,
                SubscriptionId = _settings.SubscriptionId,
                ApplicationId = _settings.ApplicationId, 
                ApplicationKey = _settings.ApplicationKey,
                WorkspaceId = _settings.WorkspaceId,
                WorkspaceKey = _settings.WorkspaceKey
            };

            if(isTested)
            {
                model.IsTested = isTested;
            }

            return model;
        }
    }
}
