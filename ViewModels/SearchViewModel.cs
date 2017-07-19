using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using laui.Models;

namespace laui.ViewModels
{
    public class SearchViewModel
    {
        public string TransactionId { get; set; }
        public string VehicleId { get; set; }

        List<Record> records { get; set; }

        public SearchViewModel()
        {
        }
    }
}