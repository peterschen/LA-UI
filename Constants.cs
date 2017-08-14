using System;
using System.Diagnostics;
using System.Reflection;

namespace laui
{
    public class Constants
    {
        public const string AzureAccountContext = "https://login.windows.net";

        public const string LogAnalyticsApiRoot = "https://management.azure.com/";
        
        // 0 == API root e.g. https://management.azure.com
        // 1 == subscription id
        // 2 == resource group
        // 3 == workspace name
        private const string LogAnalyticsApiString = "{0}/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.OperationalInsights/workspaces/{3}/api/query?api-version=2017-01-01-preview";

        public static object LogAnalyticsSearchProperties = new { 
            Options = new {
                deferpartialqueryfailures = true
            }
        };

        public const string LogAnalyticsSearchQueryPrefix = "set truncationmaxrecords=10000;set truncationmaxsize=67108864";

        public const string ContentTypeJson = "application/json";
        public const string ContentTypeText = "text/plain";
        public const string ContentTypeAll = "*/*";

        public const string RecordTypeTrace = "LauiTrace";
        public const string RecordTypeTest = "LauiTest";

        public static Lazy<string> ProductVersion = new Lazy<string>(() =>
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            return assembly.ImageRuntimeVersion;
        });

        public static Lazy<string> ProductName = new Lazy<string>(() =>
        {
            return "LA-UI";
        });

        public static Lazy<string> UserAgent = new Lazy<string>(() =>
        {
            return string.Format("{0}/{1}", ProductName, ProductVersion);
        });

        public static string LogAnalyticsApiSearch(string subscriptionId, string resourceGroup, string workspaceName)
        {
            return string.Format(LogAnalyticsApiString, LogAnalyticsApiRoot, subscriptionId, resourceGroup, workspaceName);
        }
    }
}