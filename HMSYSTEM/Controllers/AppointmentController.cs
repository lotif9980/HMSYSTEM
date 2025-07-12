using HMSYSTEM.Enum;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        protected readonly IUnitOfWork _unitofWork;

        public AppointmentController(IUnitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }

        [Authorize]
        public IActionResult Index()
        {
           var appointments= _unitofWork.AppointmentRepository.GetAllAppointments();
            return View(appointments);
        }

        [HttpGet]
        public IActionResult Save()
        {
            var department =_unitofWork.departmentRepo.getAll()
                .Where(c => c.Status == true)
                .ToList();
            var doctor = _unitofWork.doctorRepo.getAll().
                Where(c => c.Status == true)
                .ToList();

            ViewBag.Department = department;
            ViewBag.Doctor = doctor;


            var lastSerial = _unitofWork.AppointmentRepository.GetSerial()
            .OrderByDescending(a => a.SerialNumber)
            .Select(a => a.SerialNumber)
            .FirstOrDefault();

            int nextSerial = (lastSerial ?? 0) + 1; 
            ViewBag.NextSerial = nextSerial;

            return View();
        }

        [HttpGet]
        public IActionResult GetPatientNameByPhone(string phoneNumber)
        {
            var patient = _unitofWork.PatienRepo.getAll()
                .FirstOrDefault(p => p.Phone == phoneNumber && p.Status == true);

            if (patient != null)
            {
                return Json(new { success = true, 
                    name = patient.FirstName+" " + patient.LastName ,
                    id=patient.PatientID
                });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult Save(Appointment appointment)
        {
            _unitofWork.AppointmentRepository.Save(appointment);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int Id)
        {
            _unitofWork.AppointmentRepository.Delete(Id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetDeleteList()
        {
            var data = _unitofWork.AppointmentRepository.GetDeleteAppointments();
            return View(data);
        }

        [HttpGet]
        public IActionResult GetProgress()
        {
            var data =_unitofWork.AppointmentRepository.GetProgress();
            return View(data);
        }

        [HttpGet]
        public IActionResult GetComplete()
        {
            var data = _unitofWork.AppointmentRepository.GetComplete();
            return View(data);
        }

        public IActionResult ChangeStatus(int id, int status)
        {
            _unitofWork.AppointmentRepository.UpdateStatus(id, (AppointmentStatus)status);
            return RedirectToAction("Index");
        }
    }
}
