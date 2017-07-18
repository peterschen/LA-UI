using System.ComponentModel.DataAnnotations;

namespace laui.Models
{
    public class SeedModel
    {
        [Required]
        [Display(Name = "Transactions (total)")]
        public int TotalTransactions { get; set; }

        [Display(Name = "Transactions (bad)")]
        public int? BadTransactions { get; set; }

        [Required]
        [Display(Name = "Vehicle ID")]
        public string VehicleId { get; set; }

        public bool IsSeeded { get; set; }
        public bool IsSuccess { get; set; }

        public SeedModel()
        {
            TotalTransactions = 20;
            BadTransactions = 0;
        }
    }
}