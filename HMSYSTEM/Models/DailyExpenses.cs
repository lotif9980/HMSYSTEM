namespace HMSYSTEM.Models
{
    public class DailyExpenses
    {
        public int Id { get;set; }
        public DateTime Date { get; set; }
        public decimal Amount {  get; set; }
        public string Description { get; set; }
        public int BudgetId { get; set; }

    }
}
