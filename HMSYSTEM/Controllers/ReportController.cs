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
            ViewBag.FromDate = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
            return View(new List<AdmissionViewModel>());            
        }

        [HttpPost]
        public IActionResult AdmissionReports(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                // যদি user date না দেয় → empty list
                ViewBag.FromDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.ToDate = DateTime.Now.ToString("yyyy-MM-dd");
                return View(new List<AdmissionViewModel>());
            }
            var data = _unitOfWork.reportRepository.GetAllAdmission(fromDate.Value, toDate.Value);


            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            return View("AdmissionReport", data);
        }
    }
}
