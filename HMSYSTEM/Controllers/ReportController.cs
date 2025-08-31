using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMSYSTEM.Controllers
{
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetWardBedStatus()
        {
            var wards = _unitOfWork.wardRepository.GetAll()
                        .Select(w => new SelectListItem
                        {
                            Value = w.Id.ToString(),
                            Text = w.Name
                        }).ToList();

            ViewBag.Ward = wards;

            return View(new List<WardBedViewModel>()); 
        }

        [HttpPost]
        public IActionResult GetWardBedStatus(int? wardId)
        {
            var wards = _unitOfWork.wardRepository.GetAll()
                        .Select(w => new SelectListItem
                        {
                            Value = w.Id.ToString(),
                            Text = w.Name
                        }).ToList();

            ViewBag.Ward = wards;

            var data = _unitOfWork.reportRepository.GetWardBedStatus(wardId);
            return View(data);
        }

        [HttpGet]
        public IActionResult AdmissionReport()
        {
            return View(new List<AdmissionViewModel>());            
        }

        [HttpPost]
        public IActionResult AdmissionReports()
        {
            var data = _unitOfWork.reportRepository.GetAllAdmission();
            return View("AdmissionReport",data);
        }
    }
}
