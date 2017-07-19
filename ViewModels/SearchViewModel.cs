using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using laui.Models;

namespace laui.ViewModels
{
    public class SearchViewModel
    {
        [Display(Name = "Transaction ID")]
        public string TransactionId { get; set; }
        
        [Display(Name = "Vehicle ID")]
        public string VehicleId { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}