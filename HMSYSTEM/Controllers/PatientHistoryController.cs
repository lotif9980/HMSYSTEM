using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class PatientHistoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientHistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var data = _unitOfWork.PatienHistoryRepo.GetAll();
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }
       
    }
}
