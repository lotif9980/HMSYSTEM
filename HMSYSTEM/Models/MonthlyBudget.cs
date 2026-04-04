using System.ComponentModel.DataAnnotations.Schema;

namespace HMSYSTEM.Models
{
    public class MonthlyBudget
    {
        public int Id { get; set; }
        public string MonthYear { get; set; }
        public decimal TotalBudget {  get; set; }
        [NotMapped]
        public string MonthName { get; set; }
    }
}
