using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

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

        #region Admission Report
        [HttpGet]
        public IActionResult AdmissionReport()
        {
            return View();            
        }

        [HttpPost]
        public IActionResult AdmissionReport(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return View(new List<AdmissionViewModel>());
            }
            var data = _unitOfWork.reportRepository.GetAllAdmission(fromDate.Value, toDate.Value);


            return View(data);
        }

        [HttpGet]
        public IActionResult GetAdmissionPrint(string fromDate , string toDate)
        {
            DateTime from = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var admission = _unitOfWork.reportRepository.GetAllAdmission(from, to);

            return PartialView("_AdmissionPrintPertial", admission);
        }

        #endregion End Admissio Report

        #region Appointment Report
        [HttpGet]
        public IActionResult AppointmentReport()
        {
            return View() ;
        }

        [HttpPost]
        public IActionResult AppointmentReports(DateTime? fromDate , DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return View(new List<AppointmentVM>());
            }
            var data = _unitOfWork.reportRepository.GetAppointment(fromDate.Value, toDate.Value);
            return View("AppointmentReport", data);
        }

        [HttpGet]
        public IActionResult GetAppointmentReport(string fromDate, string toDate)
        {
            DateTime from = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var appointments = _unitOfWork.reportRepository.GetAppointment(from, to);

            return PartialView("_AppointmentPrintPertial", appointments);
        }

        #endregion


        #region Prescription Report

        [HttpGet]
        public IActionResult PrescriptionReports()
        {
            return View();
        }

        [HttpPost]
        public IActionResult PrescriptionReport(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return View(new List<PrescriptionViewModel>());
            }
            var data = _unitOfWork.reportRepository.GetPrescriptions(fromDate.Value, toDate.Value);
            return View("PrescriptionReports", data);
        }

        [HttpGet]
        public IActionResult GetPrescriptionPertial(string fromDate, string toDate)
        {
            DateTime from = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var prescription=_unitOfWork.reportRepository.GetPrescriptions(from, to);

           return PartialView("_PrescriptionPrintPertial",prescription);
        }
#endregion Prescription Report


        #region Bill Report
        [HttpGet]
        public IActionResult BillReport()
        {
            return View();
        }


        [HttpPost]
        public IActionResult BillReports(DateTime? fromDate, DateTime? toDate)
        {
            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return View(new List<BillViewModel>());
            }
            var data = _unitOfWork.reportRepository.GetBill(fromDate.Value, toDate.Value);
            return View("BillReport", data);
        }

        public IActionResult PrintGetBills(string fromDate, string toDate)
        {
            DateTime from = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime to = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var bill =_unitOfWork.reportRepository.GetBill(from, to);

            return PartialView("_BillsPrintPartial", bill);
        }
        #endregion
    }
}
