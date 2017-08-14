using System;

namespace laui.Models
{
    public class ConfigurationModel
    {
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationKey { get; set; }
        public string ResourceGroup { get; set; }
        public string WorkspaceId { get; set; }
        public string WorkspaceKey { get; set; }
        public string WorkspaceName { get; set; }

        public bool IsConfigured
        {
            get {
                return !string.IsNullOrEmpty(TenantId) &&
                    !string.IsNullOrEmpty(SubscriptionId) &&
                    !string.IsNullOrEmpty(ApplicationId) &&
                    !string.IsNullOrEmpty(ApplicationKey) &&
                    !string.IsNullOrEmpty(ResourceGroup) &&
                    !string.IsNullOrEmpty(WorkspaceId) &&
                    !string.IsNullOrEmpty(WorkspaceKey) &&
                    !string.IsNullOrEmpty(WorkspaceName);
            }
        }
        public bool IsTested { get; set; }
        public bool IsValid { get; set; }

        public ConfigurationModel()
        {
            IsTested = false;
            IsValid = false;
        }
    }
}