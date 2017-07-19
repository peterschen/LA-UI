using System;

namespace laui.Models
{
    public class TraceRecord : Record
    {
        public Guid TransactionId { get; set; }
        public string VehicleId { get; set; }
        public string Hop { get; set; }
    }
}