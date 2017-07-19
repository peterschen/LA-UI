using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using laui.Models;

namespace laui
{
    public class LogSearcher
    {
        private string _tenantId;
        private string _applicationId;
        private string _applicationKey;
        private string _subscriptionId;
        private string _resourceGroup;
        private string _workspaceName;

        private string _token = null;

        public LogSearcher(string tenantId, string applicationId, string applicationKey, string subscriptionId, string resourceGroup, string workspaceName)
        {
            _tenantId = tenantId;
            _applicationId = applicationId;
			_applicationKey = applicationKey;
            _subscriptionId = subscriptionId;
            _resourceGroup = resourceGroup;
            _workspaceName = workspaceName;
        }

        public bool SearchTestRecords()
        {
            string result = null;
            try
            {
                var task = Search("*");
                task.Wait();
                result = task.Result;
            }
            catch
            {
                // swallow exception
            }

            if(result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> SearchTestRecordsAsync()
        {
            var result = await Search("*");
            if(result != null)
            {
                return true;
            }

            return false;
        }

        public List<Transaction> SearchTransactions(string transactionId, string vehicleId)
        {
            var records = SearchTraceRecords(transactionId, vehicleId);
            return Transaction.Parse(records);
        }
        
        public List<TraceRecord> SearchTraceRecords(string transactionId, string vehicleId)
        {
            if(string.IsNullOrEmpty(transactionId) && string.IsNullOrEmpty(vehicleId))
            {
                return new List<TraceRecord>();
            }

            /*
                var query = "LauiTrace_CL";
                query += "| where Timestamp_t > ago(3d)";
                if(!string.IsNullOrEmpty(transactionId))
                {
                    query += string.Format("| where TransactionId_g == '{0}'", transactionId);
                }
                if(!string.IsNullOrEmpty(vehicleId))
                {
                    query += string.Format("| where VehicleId_s == '{0}'", vehicleId);
                }
                query += "| order by Timestamp_t DESC";

                var responseString = await Search(query, null, null);
            */
            
            if(!string.IsNullOrEmpty(vehicleId))
            {
                return RecordFactory.GenerateTraceRecords(vehicleId, 15, 3);
            }

            return new List<TraceRecord>();
        }

        private async Task<string> Search(string query, DateTime? start = null, DateTime? end = null)
        {
            if(!start.HasValue)
            {
                start = DateTime.Now.AddDays(-1);
            }

            if(!end.HasValue)
            {
                end = DateTime.Now;
            }

            var endpoint = string.Format(Constants.LogAnalyticsSearchEndpoint, _subscriptionId, _resourceGroup, _workspaceName);
            var parameter = JsonConvert.SerializeObject(new
            {
                query = query,
                start = start.Value.ToUniversalTime(),
                end = end.Value.ToUniversalTime()
            });

            return await ReadData(endpoint, parameter);
        }

        private async Task<string> ReadData(string endpoint, string payload = "")
        {
            GetAccessToken();

            Uri uri = new Uri(new Uri("https://management.azure.com"), endpoint);
            StringContent content = new StringContent(payload, Encoding.UTF8, Constants.JsonContentType);
            string responseString = null;

            using(var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                client.DefaultRequestHeaders.Add("User-Agent", Constants.UserAgent.Value);
                client.DefaultRequestHeaders.Add("Accept", Constants.JsonContentType);

                using(HttpResponseMessage response = await client.PostAsync(uri, content)) 
                {
                    if(response.IsSuccessStatusCode)
                    {
                        responseString = await response.Content.ReadAsStringAsync();
                    }
                }
            }

            return responseString;
        }
        private void GetAccessToken()
        {
            if(_token == null)
            {
                string authContextURL = "https://login.windows.net/" + _tenantId;
                var authenticationContext = new AuthenticationContext(authContextURL);
                var credential = new ClientCredential(clientId: _applicationId, clientSecret: _applicationKey);
                
                var task = authenticationContext.AcquireTokenAsync("https://management.azure.com/", credential);
                task.Wait();

                var result = task.Result;
                if (result == null)
                {
                    throw new InvalidOperationException("Authentication failed");
                }

                _token = result.AccessToken;
            }
        }
    }
}