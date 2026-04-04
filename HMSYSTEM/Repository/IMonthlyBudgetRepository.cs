using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IMonthlyBudgetRepository
    {
        public List<MonthlyBudget> GetAll();
        public void Add(MonthlyBudget budget);
    }
}
