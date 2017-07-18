using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace laui
{
    public class LogCollector
    {
        private string _workspaceId;
        private string _workspaceKey;
		private byte[] _workspaceKeyBytes;

        public LogCollector(string workspaceId, string workspaceKey)
        {
            _workspaceId = workspaceId;
            _workspaceKey = workspaceKey;
			_workspaceKeyBytes = Convert.FromBase64String(workspaceKey);
        }

        public bool WriteTestData()
        {
            try
            {
                Task<bool> task = WriteTestDataAsync();
                task.Wait();
                return task.Result;
            }
            catch
            {
                // swallow exception
            }
            
            return false;
        }

        public async Task<bool> WriteTestDataAsync()
        {
            var testData = new {
                LauiTest = "true"
            };

            return await WriteData(Constants.RecordTypeTest, testData);
        }

        private async Task<bool> WriteData(string logType, object data)
        {
            return await Collect(logType, data);
        }

        /// <summary>
        /// Collect a JSON log to Azure Log Analytics
        /// </summary>
        /// <param name="LogType">Name of the Type of Log. Can be any name you want to appear on Azure Log Analytics.</param>
        /// <param name="ObjectToSerialize">Object to serialize and collect.</param>
        /// <param name="ApiVersion">Optional. Api Version.</param>
        public async Task<bool> Collect(string LogType, object ObjectToSerialize, string ApiVersion="2016-04-01", string timeGeneratedPropertyName = null)
        {
            return await Collect(LogType, Newtonsoft.Json.JsonConvert.SerializeObject(ObjectToSerialize), ApiVersion, timeGeneratedPropertyName);
        }

        /// <summary>
        /// Collect a JSON log to Azure Log Analytics
        /// </summary>
        /// <param name="LogType">Name of the Type of Log. Can be any name you want to appear on Azure Log Analytics.</param>
        /// <param name="JsonPayload">JSON string. Can be an array or single object.</param>
        /// <param name="ApiVersion">Optional. Api Version.</param>
        public async Task<bool> Collect(string LogType, string JsonPayload, string ApiVersion="2016-04-01", string timeGeneratedPropertyName = null)
        {
            var utf8Encoding = new UTF8Encoding();
            Byte[] content = utf8Encoding.GetBytes(JsonPayload);

            string url = "https://" + _workspaceId + ".ods.opinsights.azure.com/api/logs?api-version=" + ApiVersion;
            var rfcDate = DateTime.Now.ToUniversalTime().ToString("r");
            var signature = HashSignature("POST", content.Length, "application/json", rfcDate, "/api/logs");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Log-Type"] = LogType;
            request.Headers["x-ms-date"] = rfcDate;
            request.Headers["Authorization"] = signature;
            if (!string.IsNullOrEmpty(timeGeneratedPropertyName))
            {
                request.Headers["time-generated-field"] = timeGeneratedPropertyName;
            }
            request.Proxy = null;
            using (Stream requestStream = await request.GetRequestStreamAsync())
            {
                requestStream.Write(content, 0, content.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
            {
                if (!response.StatusCode.Equals(HttpStatusCode.OK) && !response.StatusCode.Equals(HttpStatusCode.Accepted))
                {
                   return false;
                }
                
                return true;
            }
        }

        private string HashSignature(string method, int contentLength, string contentType, string date, string resource)
        {
            var stringtoHash = method + "\n" + contentLength + "\n" + contentType + "\nx-ms-date:" + date + "\n" + resource;
            var encoding = new System.Text.ASCIIEncoding();
            var bytesToHash = encoding.GetBytes(stringtoHash);            
            using (var sha256 = new HMACSHA256(_workspaceKeyBytes))
            {
                var calculatedHash = sha256.ComputeHash(bytesToHash);
                var stringHash = Convert.ToBase64String(calculatedHash);
                return "SharedKey " + _workspaceId + ":" + stringHash;
            }
        }
    }
}