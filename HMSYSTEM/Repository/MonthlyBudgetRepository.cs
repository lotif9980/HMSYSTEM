using HMSYSTEM.Data;
using HMSYSTEM.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HMSYSTEM.Repository
{
    public class MonthlyBudgetRepository : IMonthlyBudgetRepository
    {

        private readonly Db _db;
        public MonthlyBudgetRepository(Db db)
        {
           _db = db;
        }

        //public List<MonthlyBudget> GetAll()
        //{
        //    var data = _db.MonthlyBudgets.ToList();

        //    foreach (var item in data)
        //    {
        //        var parts = item.MonthYear.Split('-');
        //        int year = int.Parse(parts[0]);
        //        int month = int.Parse(parts[1]);

        //        item.MonthName = new DateTime(year, month, 1).ToString("MMMM");
        //    }

        //    return data;
        //}

        public List<MonthlyBudget> GetAll()
        {
            return _db.MonthlyBudgets
                .Select(item => new MonthlyBudget
                {
                    Id = item.Id,
                    MonthYear = item.MonthYear,
                    TotalBudget = item.TotalBudget,
                    MonthName = DateTime.Parse(item.MonthYear + "-01").ToString("MMMM")
                })
                .ToList();
        }

        public void Add(MonthlyBudget budget)
        {
           _db.MonthlyBudgets.Add(budget);
        }

    }
}
