using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using laui.Models;

namespace laui
{
    public class RecordFactory
    {
        private static readonly Random _random = new Random();
        
        public static string GenerateTraceRecordString(string vehicleId, int totalTransactions = 20, int badTransactions = 0, string[] hops = null)
        {
            var records = GenerateTraceRecords(vehicleId, totalTransactions, badTransactions, hops);
            return JsonConvert.SerializeObject(records);
        }

        public static List<TraceRecord> GenerateTraceRecords(string vehicleId, int totalTransactions = 20, int badTransactions = 0, string[] hops = null)
        {
            int badTransactionStep = -1;
            if(badTransactions > 0)
            {
        	    badTransactionStep = (int) Math.Round((double) (totalTransactions / badTransactions));
            }

            if(hops == null)
            {
                hops = new string[] { "Vehicle", "IoT Hub", "Event Hub (Location)", "Service Fabric (Business Logic)", "Service Fabric (Vehicle Actor)" };
            }

            List<TraceRecord> records = new List<TraceRecord>();

            for(var i = 1; i <= totalTransactions; i++)
            {
                int badHop = -1;
                var transactionId = Guid.NewGuid();
                var timestamp = DateTime.Now;

                if(badTransactionStep > 0 && i % badTransactionStep == 0)
                {
                    badHop = _random.Next(0, hops.Length - 1);
                }

                for(var j  = 0; j < hops.Length; j++)
                {
                    if(badHop == -1 || j < badHop)
                    {
                        records.Add(new TraceRecord {
                            Timestamp = timestamp,
                            TransactionId = transactionId,
                            VehicleId = vehicleId,
                            Hop = hops[j]
                        });

                        timestamp = timestamp.AddSeconds(_random.Next(0, 30));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return records;
        }
    }
}