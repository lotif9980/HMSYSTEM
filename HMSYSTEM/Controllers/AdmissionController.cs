using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class AdmissionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdmissionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var data = _unitOfWork.admissionRepository.getAll();
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
        {
            var doctors = _unitOfWork.doctorRepo.getAll();
            ViewBag.Doctor = doctors;

            var beds=_unitOfWork.bedRepository.getAllBed().ToList().Where(p=>p.IsOccupied==true);
            ViewBag.Bed = beds;

            var wards = _unitOfWork.wardRepository.GetAll().ToList();
            ViewBag.Ward= wards;

            int lastSerial = _unitOfWork.admissionRepository.GetLastInvoiceNo();
            int nextSerial = lastSerial + 1;
            ViewBag.NextSerial = nextSerial;


            return View();
        }

        [HttpPost]
        public IActionResult Save(Admission admission)
        {
            _unitOfWork.admissionRepository.Save(admission);

            return RedirectToAction("Index");
        }

        public IActionResult GetPatientPhoneNumber(string phoneNumber)
        {
          var patient= _unitOfWork.PatienRepo.getAll().FirstOrDefault(p=>p.Phone== phoneNumber && p.Status==true);

            if (patient != null)
            {
                return Json(new
                {
                    success = true,
                    name = patient.FirstName + " " + patient.LastName,
                    fName = patient.FatherName,
                    id = patient.PatientID

                });
            }
            else
            {
                return Json(new{success = false});
            }
        }

        [HttpGet]
        public IActionResult GetBedsByWardId(int wardId)
        {
            var beds = _unitOfWork.bedRepository.getAllBed()
                .Where(b => b.WardId == wardId)
                .Select(b => new { b.Id, b.BedNumber })
                .ToList();

            return Json(beds);
        }

    }
}
