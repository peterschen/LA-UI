using System.ComponentModel.DataAnnotations;

namespace laui.ViewModels
{
    public class SeedViewModel
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

        public SeedViewModel()
        {
            TotalTransactions = 20;
            BadTransactions = 0;
        }
    }
}