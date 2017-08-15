using System;
using Newtonsoft.Json.Linq;

namespace laui.Models
{
    public class TraceRecord : Record
    {
        public Guid TransactionId { get; set; }
        public string VehicleId { get; set; }
        public string Hop { get; set; }

        public static TraceRecord Parse(JToken token)
        {
            return new TraceRecord()
            {
            };
        }
    }
}