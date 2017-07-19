using System;
using System.Collections.Generic;
using System.Linq;

namespace laui.Models
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public string VehicleId { get; set; }

        public DateTime Timestamp
        {
            get
            {
                var record = Records.OrderBy(r => r.Timestamp).FirstOrDefault();
                if(record == null)
                {
                    return DateTime.MinValue;
                }
                else
                {
                    return record.Timestamp;
                }
            }
        }

        public List<TraceRecord> Records { get; set; }
        public bool IsCompleted { get; set; }

        public Transaction(Guid transactionId, string vehicleId)
        {
            TransactionId = transactionId;
            VehicleId = vehicleId;
            Records = new List<TraceRecord>();
        }

        public static List<Transaction> Parse(List<TraceRecord> records, int hops = 5)
        {
            var transactions = new Dictionary<Guid, Transaction>();

            foreach(var record in records)
            {
                Transaction transaction;
                if(!transactions.ContainsKey(record.TransactionId))
                {
                    transaction = new Transaction(record.TransactionId, record.VehicleId);
                    transactions.Add(record.TransactionId, transaction);
                }
                else
                {
                    transaction = transactions[record.TransactionId];
                }

                transaction.Records.Add(record);
                if(transaction.Records.Count == hops)
                {
                    transaction.IsCompleted = true;
                }
            }

            return transactions.Values.ToList();
        }
    }
}