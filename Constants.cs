using System;
using System.Diagnostics;
using System.Reflection;

namespace laui
{
    public class Constants
    {
        public const string LogAnalyticsSearchEndpoint = "/subscriptions/{0}/resourceGroups/{1}/providers/Microsoft.OperationalInsights/workspaces/{2}/search?api-version=2015-03-20";

        public const string JsonContentType = "application/json";

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