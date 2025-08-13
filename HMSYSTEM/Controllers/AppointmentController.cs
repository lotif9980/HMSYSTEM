using HMSYSTEM.Enum;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
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
        public IActionResult Index(int pageSize=10, int page=1)
        {
            var totalAappointments= _unitofWork.AppointmentRepository.GetAllAppointments();
            var totalItem=totalAappointments.Count();
            var totalPage=(int)Math.Ceiling((decimal)totalItem/pageSize);
            var appointment= totalAappointments
                             .Skip((page-1)*pageSize)
                             .Take(pageSize)
                             .ToList();

            var viewModel = new PaginationViewModel<Appointment>
            {
                Items=appointment,
                TotalItems=totalItem,
                TotalPages=totalPage,
                CurrentPage=page,
                PageSize=pageSize
            };
            return View(viewModel);
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

            return Json(new { success = false  });
        }

        [HttpPost]
        public IActionResult Save(Appointment appointment)
        {
            _unitofWork.AppointmentRepository.Save(appointment);
            TempData["Message"] = "✅ Successfully Added!";
            TempData["MessageType"] = "primary";


            return RedirectToAction("Save");
        }

        public IActionResult Delete(int Id)
        {
            _unitofWork.AppointmentRepository.Delete(Id);
            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetDeleteList(int page = 1, int pageSize = 10)
        {
            var totalDelete = _unitofWork.AppointmentRepository.GetDeleteAppointments();
            var totalItem = totalDelete.Count();
            var totalPage = (int)Math.Ceiling((decimal)totalItem / pageSize);
            var progress = totalDelete
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();

            var viewModel = new PaginationViewModel<Appointment>
            {
                Items = progress,
                TotalItems = totalItem,
                TotalPages = totalPage,
                CurrentPage = page,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetProgress(int page=1 , int pageSize=10)
        {
            var totalProgress =_unitofWork.AppointmentRepository.GetProgress();
            var totalItem = totalProgress.Count();
            var totalPage = (int)Math.Ceiling((decimal)totalItem / pageSize);
            var progress=totalProgress
                          .Skip((page-1)*pageSize)
                          .Take(pageSize)
                          .ToList();

            var viewModel = new PaginationViewModel<Appointment>
            {
               Items= progress,
               TotalItems= totalItem,
               TotalPages= totalPage,
               CurrentPage= page,
               PageSize= pageSize
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetComplete(int page = 1, int pageSize = 10)
        {
            var totalComplete = _unitofWork.AppointmentRepository.GetComplete();
            var totalItem = totalComplete.Count();
            var totalPage = (int)Math.Ceiling((decimal)totalItem / pageSize);
            var progress = totalComplete
                          .Skip((page - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();

            var viewModel = new PaginationViewModel<Appointment>
            {
                Items = progress,
                TotalItems = totalItem,
                TotalPages = totalPage,
                CurrentPage = page,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        public IActionResult ChangeStatus(int id, int status)
        {
            _unitofWork.AppointmentRepository.UpdateStatus(id, (AppointmentStatus)status);
            return RedirectToAction("Index");
        }
    }
}
