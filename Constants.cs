using System;
using System.Diagnostics;
using System.Reflection;

namespace laui
{
    public class Constants
    {
        public const string LogAnalyticsSearchEndpoint = "https://api.loganalytics.io/beta/workspaces/{0}/query?timestamp={1}";

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
    }
}