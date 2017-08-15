using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
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
        private string _workspaceId;
        private string _workspaceName;

        private string _token = null;

        public LogSearcher(
            string tenantId, string applicationId, string applicationKey, 
            string subscriptionId, string resourceGroup, string workspaceId, string workspaceName)
        {
            _tenantId = tenantId;
            _applicationId = applicationId;
			_applicationKey = applicationKey;
            _subscriptionId = subscriptionId;
            _resourceGroup = resourceGroup;
            _workspaceId = workspaceId;
            _workspaceName = workspaceName;
        }

        public bool SearchTestRecords()
        {
            string result = null;
            try
            {
                var task = Search("search * | take 1");
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
            var result = await Search("search * | take 1");
            if(result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<List<Transaction>> SearchTransactions(string transactionId, string vehicleId)
        {
            var records = await SearchTraceRecords(transactionId, vehicleId);
            return Transaction.Parse(records);
        }
        
        public async Task<List<TraceRecord>> SearchTraceRecords(string transactionId, string vehicleId)
        {
            var query = "LauiTrace_CL";
            query += "| where TimeGenerated > ago(3d)";
            if(!string.IsNullOrEmpty(transactionId))
            {
                query += string.Format("| where TransactionId_g contains '{0}'", transactionId);
            }
            if(!string.IsNullOrEmpty(vehicleId))
            {
                query += string.Format("| where VehicleId_s contains '{0}'", vehicleId);
            }
            query += "| project TransactionId = TransactionId_g, VehicleId = VehicleId_s, Hop = Hop_s, Timestamp = Timestamp_t";
            query += "| order by Timestamp desc";

            var responseString = await Search(query);
            var records = new List<TraceRecord>();

            if(responseString != null)
            {
                JObject result = JObject.Parse(responseString);
                records = result.SelectTokens("Tables[?(@.TableName == 'Table_0')].Rows[*]")
                    .Select(t => new TraceRecord() { 
                        TransactionId = Guid.Parse(t.FirstOrDefault().ToString()),
                        VehicleId = t.Skip(1).FirstOrDefault().ToString(),
                        Hop = t.Skip(2).FirstOrDefault().ToString(),
                        Timestamp = DateTime.Parse(t.LastOrDefault().ToString())
                    }
                ).ToList();
            }

            return records;
        }

        private async Task<string> Search(string query, DateTime? start = null, DateTime? end = null)
        {
            string timespan = "PT24H";
            if(!start.HasValue && !end.HasValue)
            {
                // default timespan assignment
            }
            else
            {
                if(!start.HasValue)
                {
                    start = DateTime.Now.AddDays(-1);
                }

                if(!end.HasValue)
                {
                    end = DateTime.Now;
                }

                timespan = string.Format("{0:O}/{1:O}", start.Value.ToUniversalTime(), end.Value.ToUniversalTime());
            }

            // query = string.Format("{0};{1}", Constants.LogAnalyticsSearchQueryPrefix, query);
            // var parameter = JsonConvert.SerializeObject(new
            // {
            //     query = query,
            //     properties = Constants.LogAnalyticsSearchProperties
            // });

            return await ReadData(query);
        }

        private async Task<string> ReadData(string query)
        {
            GetAccessToken();

            Uri uri = new Uri(Constants.LogAnalyticsApiSearch(_subscriptionId, _resourceGroup, _workspaceName, query));
            // StringContent content = new StringContent(payload, Encoding.UTF8, Constants.ContentTypeJson);
            string responseString = null;

            // var handler = new HttpClientHandler {
            //     UseDefaultCredentials = false,
            //     Proxy = new DebugProxy("http://localhost:8888"),
            //     UseProxy = true
            // };

            using(var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentTypeJson));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentTypeText));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Constants.ContentTypeAll));
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.ProductName.Value, Constants.ProductVersion.Value));

                using(HttpResponseMessage response = await client.GetAsync(uri)) 
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
                string authContextURL = string.Format("{0}/{1}", Constants.AzureAccountContext, _tenantId);
                var authenticationContext = new AuthenticationContext(authContextURL);
                var credential = new ClientCredential(clientId: _applicationId, clientSecret: _applicationKey);
                
                var task = authenticationContext.AcquireTokenAsync(Constants.LogAnalyticsApiRoot, credential);
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