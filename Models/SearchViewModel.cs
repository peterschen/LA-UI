using System.ComponentModel.DataAnnotations;

namespace laui.Models
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