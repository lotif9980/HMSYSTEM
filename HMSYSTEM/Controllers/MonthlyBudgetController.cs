using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class MonthlyBudgetController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public MonthlyBudgetController(IUnitOfWork unitOfWork)
        {
          _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var data =_unitOfWork.monthlyBudgetRepository.GetAll();
            
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]  
        public IActionResult Save(MonthlyBudget monthlyBudget)
        {

            _unitOfWork.monthlyBudgetRepository.Add(monthlyBudget);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }


    }
}
