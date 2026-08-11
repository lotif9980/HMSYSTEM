using HMSYSTEM.Enum;
using HMSYSTEM.Helpers;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static HMSYSTEM.Helpers.QueryableExtensions;

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

        private int CalculateNextSerial()
        {
            var lastSerial = _unitofWork.AppointmentRepository.GetSerial()
                            .OrderByDescending(a => a.SerialNumber)
                            .Select(a => a.SerialNumber)
                            .FirstOrDefault();

            return (lastSerial == null || lastSerial == 0) ? 1001 : (lastSerial.Value + 1);
        }

        [HttpGet]
        public IActionResult GetNextSerial()
        {
            int nextSerial = CalculateNextSerial();
            return Json(new { nextSerial = nextSerial });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            DateTime fromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime toDate = fromDate.AddMonths(1).AddDays(-1);

            ViewBag.FromDate = fromDate.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.ToString("yyyy-MM-dd");

            ViewBag.Department = _unitofWork.departmentRepo.getAll().Where(c => c.Status == true).ToList();
            ViewBag.Doctor = _unitofWork.doctorRepo.getAll().Where(c => c.Status == true).ToList();

            ViewBag.NextSerial = CalculateNextSerial();

            return View();
        }

        [HttpGet]
        public IActionResult GetAppointments(string search = "", string fromDate = "", string toDate = "", int page = 1, int pageSize = 10)
        {
            int roleId = Helper.GetRoleId(User);
            int doctorId = Helper.GetDoctorId(User);

            DateTime fDate = string.IsNullOrEmpty(fromDate)
                ? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
                : DateTime.Parse(fromDate);
            DateTime tDate = string.IsNullOrEmpty(toDate)
                ? fDate.AddMonths(1).AddDays(-1)
                : DateTime.Parse(toDate);

            IQueryable<Appointment> query;

            if (roleId == (int)RoleEnum.Doctor && doctorId > 0)
            {
                query = _unitofWork.AppointmentRepository.GetAppointmentsByDoctorId(doctorId, fDate, tDate);
            }
            else
            {
                query = _unitofWork.AppointmentRepository.GetAppointmentsByDoctorId(null, fDate, tDate);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(a =>
                    (a.Patient != null && (
                        (!string.IsNullOrEmpty(a.Patient.FirstName) && a.Patient.FirstName.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(a.Patient.LastName) && a.Patient.LastName.ToLower().Contains(search))
                    )) ||
                    (!string.IsNullOrEmpty(a.PatientPhoneNumber) && a.PatientPhoneNumber.Contains(search)) ||
                    (a.Doctor != null && !string.IsNullOrEmpty(a.Doctor.FirstName) && a.Doctor.FirstName.ToLower().Contains(search)) ||
                    (a.Department != null && !string.IsNullOrEmpty(a.Department.DepartmentName) && a.Department.DepartmentName.ToLower().Contains(search))
                );
            }

            query = query.OrderByDescending(a => a.AppointmentId);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(a => new
                {
                    appointmentId = a.AppointmentId,
                    date = a.AppoinmentDate.ToString("dd-MM-yyyy"),
                    time = a.AppoinmentDate.ToString("hh:mm tt"),
                    serialNumber = a.SerialNumber,
                    patientName = a.Patient != null ? (a.Patient.FirstName) : "N/A",
                    phone = a.PatientPhoneNumber,
                    department = a.Department != null ? a.Department.DepartmentName : "N/A",
                    doctor = a.Doctor != null ? a.Doctor.FirstName: "N/A",
                    status = (int)a.Status
                })
                .ToList();

            return Json(new
            {
                issuccess = true,
                totalPages,
                totalItem,
                currentPage = page,
                data
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(int? doctroId, DateTime? fromDate, DateTime? toDate, int pageSize = 10, int page = 1)
        {
            IQueryable<Appointment> query;

            int roleId = Helper.GetRoleId(User);
            int doctorId = Helper.GetDoctorId(User);

            if (roleId == (int)RoleEnum.Doctor && doctorId > 0)
            {
                query = _unitofWork.AppointmentRepository.GetAppointmentsByDoctorId(doctorId, fromDate, toDate);
            }
            else
            {
                query=_unitofWork.AppointmentRepository.GetAppointmentsByDoctorId(null, fromDate, toDate);
            }
       

            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd") ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");

            var pagedData = query
                .OrderBy(a => a.AppointmentId)
                .ToPagedList(page, pageSize);

            return View(pagedData);
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
            ViewBag.NextSerial = CalculateNextSerial();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPatientNameByPhone(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return Json(new { success = false });
            }

            phoneNumber = phoneNumber.Trim();

            var patient = _unitofWork.PatienRepo.getAll()
                .FirstOrDefault(p => p.Phone != null && p.Phone.Trim() == phoneNumber && p.Status == true);

            if (patient != null)
            {
                var isAppointment = await _unitofWork.AppointmentRepository.AppointmentCheck(patient.PatientID);

                if (isAppointment)
                {
                    return Json(new
                    {
                        success = false,
                        alreadyAdded = true,
                        name = patient.FirstName + " " + patient.LastName,
                        id = patient.PatientID
                    });
                }
                return Json(new
                {
                    success = true,
                    name = patient.FirstName + " " + patient.LastName,
                    id = patient.PatientID
                });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult Save(AppointmentVM appointment)
        {
            ViewBag.Department = _unitofWork.departmentRepo.getAll();
            ViewBag.Doctor = _unitofWork.doctorRepo.getAll();
            ViewBag.NextSerial = CalculateNextSerial();

            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                var patient = GetPatientNameByPhone(appointment.PatientPhoneNumber);
                if (patient != null)
                {
                    appointment.PatientName = appointment.PatientName; 
                }

                Appointment appointments = new Appointment
                {
                    PatientID = appointment.PatientID,
                    PatientPhoneNumber = appointment.PatientPhoneNumber,
                    DepartmentId = appointment.DepartmentId,
                    DoctorId = appointment.DoctorId,
                    AppoinmentDate = appointment.AppoinmentDate.Value,
                    SerialNumber = appointment.SerialNumber > 0 ? appointment.SerialNumber : CalculateNextSerial(),
                    Problem = appointment.Problem,
                    Status = AppointmentStatus.Active
                };

                _unitofWork.AppointmentRepository.Save(appointments);
                TempData["Message"] = "✅ Successfully Added!";
                TempData["MessageType"] = "primary";

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully Added!" });
                }

                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid data submitted." });
            }

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int Id)
        {
            _unitofWork.AppointmentRepository.Delete(Id);
            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetDeleteList()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetDeleteListData(string search = "", int page = 1, int pageSize = 10)
        {
            int roleId = Helper.GetRoleId(User);
            int doctorId = Helper.GetDoctorId(User);

            IQueryable<Appointment> query;

            if (roleId == (int)RoleEnum.Doctor && doctorId > 0)
                query = _unitofWork.AppointmentRepository.GetDeleteAppointments(doctorId);
            else
                query = _unitofWork.AppointmentRepository.GetDeleteAppointments();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(a =>
                    (a.Patient != null && (a.Patient.FirstName.ToLower().Contains(search) || a.Patient.LastName.ToLower().Contains(search))) ||
                    (!string.IsNullOrEmpty(a.PatientPhoneNumber) && a.PatientPhoneNumber.Contains(search)) ||
                    (a.Doctor != null && a.Doctor.FirstName.ToLower().Contains(search)) ||
                    (a.Department != null && a.Department.DepartmentName.ToLower().Contains(search))
                );
            }

            query = query.OrderByDescending(a => a.AppointmentId);
            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);
            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(a => new {
                    appointmentId = a.AppointmentId,
                    date = a.AppoinmentDate.ToString("dd-MM-yyyy"),
                    time = a.AppoinmentDate.ToString("hh:mm tt"),
                    serialNumber = a.SerialNumber,
                    patientName = a.Patient != null ? (a.Patient.FirstName ) : "N/A",
                    phone = a.PatientPhoneNumber,
                    department = a.Department != null ? a.Department.DepartmentName : "N/A",
                    doctor = a.Doctor != null ? (a.Doctor.FirstName) : "N/A",
                    status = (int)a.Status
                }).ToList();

            return Json(new { issuccess = true, totalPages, totalItem, currentPage = page, data });
        }

        [HttpGet]
        public IActionResult GetProgress()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetProgressData(string search = "", int page = 1, int pageSize = 10)
        {
            int roleId = Helper.GetRoleId(User);
            int doctorId = Helper.GetDoctorId(User);

            IQueryable<Appointment> query;

            if (roleId == (int)RoleEnum.Doctor && doctorId > 0)
                query = _unitofWork.AppointmentRepository.GetProgress(doctorId);
            else
                query = _unitofWork.AppointmentRepository.GetProgress();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(a =>
                    (a.Patient != null && (a.Patient.FirstName.ToLower().Contains(search) || a.Patient.LastName.ToLower().Contains(search))) ||
                    (!string.IsNullOrEmpty(a.PatientPhoneNumber) && a.PatientPhoneNumber.Contains(search)) ||
                    (a.Doctor != null && a.Doctor.FirstName.ToLower().Contains(search)) ||
                    (a.Department != null && a.Department.DepartmentName.ToLower().Contains(search))
                );
            }

            query = query.OrderByDescending(a => a.AppointmentId);
            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);
            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(a => new {
                    appointmentId = a.AppointmentId,
                    date = a.AppoinmentDate.ToString("dd-MM-yyyy"),
                    time = a.AppoinmentDate.ToString("hh:mm tt"),
                    serialNumber = a.SerialNumber,
                    patientName = a.Patient != null ? (a.Patient.FirstName ) : "N/A",
                    phone = a.PatientPhoneNumber,
                    department = a.Department != null ? a.Department.DepartmentName : "N/A",
                    doctor = a.Doctor != null ? (a.Doctor.FirstName + " " + a.Doctor.LastName) : "N/A",
                    status = (int)a.Status
                }).ToList();

            return Json(new { issuccess = true, totalPages, totalItem, currentPage = page, data });
        }

        [HttpGet]
        public IActionResult GetComplete()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetCompleteData(string search = "", int page = 1, int pageSize = 10)
        {
            int roleId = Helper.GetRoleId(User);
            int doctorId = Helper.GetDoctorId(User);

            IQueryable<Appointment> query;

            if (roleId == (int)RoleEnum.Doctor && doctorId > 0)
                query = _unitofWork.AppointmentRepository.GetComplete(doctorId);
            else
                query = _unitofWork.AppointmentRepository.GetComplete();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(a =>
                    (a.Patient != null && (a.Patient.FirstName.ToLower().Contains(search) || a.Patient.LastName.ToLower().Contains(search))) ||
                    (!string.IsNullOrEmpty(a.PatientPhoneNumber) && a.PatientPhoneNumber.Contains(search)) ||
                    (a.Doctor != null && a.Doctor.FirstName.ToLower().Contains(search)) ||
                    (a.Department != null && a.Department.DepartmentName.ToLower().Contains(search))
                );
            }

            query = query.OrderByDescending(a => a.AppointmentId);
            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);
            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(a => new {
                    appointmentId = a.AppointmentId,
                    date = a.AppoinmentDate.ToString("dd-MM-yyyy"),
                    time = a.AppoinmentDate.ToString("hh:mm tt"),
                    serialNumber = a.SerialNumber,
                    patientName = a.Patient != null ? (a.Patient.FirstName ) : "N/A",
                    phone = a.PatientPhoneNumber,
                    department = a.Department != null ? a.Department.DepartmentName : "N/A",
                    doctor = a.Doctor != null ? (a.Doctor.FirstName + " " + a.Doctor.LastName) : "N/A",
                    status = (int)a.Status
                }).ToList();

            return Json(new { issuccess = true, totalPages, totalItem, currentPage = page, data });
        }

        public IActionResult ChangeStatus(int id, int status, string returnAction)
        {
            _unitofWork.AppointmentRepository.UpdateStatus(id, (AppointmentStatus)status);
            return RedirectToAction(returnAction);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var appointment = _unitofWork.AppointmentRepository.GetById(id);
            if (appointment == null)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "Appointment not found." });
                }
                return NotFound();
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        appointmentId = appointment.AppointmentId,
                        patientID = appointment.PatientID,
                        patientName = appointment.Patient != null ? (appointment.Patient.FirstName) : "",
                        patientPhoneNumber = appointment.PatientPhoneNumber,
                        departmentId = appointment.DepartmentId,
                        doctorId = appointment.DoctorId,
                        appoinmentDate = appointment.AppoinmentDate.ToString("yyyy-MM-ddTHH:mm"),
                        serialNumber = appointment.SerialNumber,
                        problem = appointment.Problem,
                        status = (int)appointment.Status
                    }
                });
            }

            var department = _unitofWork.departmentRepo.getAll()
                .Where(c => c.Status == true)
                .ToList();
            var doctor = _unitofWork.doctorRepo.getAll()
                .Where(c => c.Status == true)
                .ToList();

            ViewBag.Department = department;
            ViewBag.Doctor = doctor;

            var model = new AppointmentVM
            {
                AppointmentId = appointment.AppointmentId,
                PatientID = appointment.PatientID,
                PatientName = appointment.Patient != null ? (appointment.Patient.FirstName + " " + appointment.Patient.LastName) : "",
                PatientPhoneNumber = appointment.PatientPhoneNumber,
                DepartmentId = appointment.DepartmentId,
                DoctorId = appointment.DoctorId,
                AppoinmentDate = appointment.AppoinmentDate,
                SerialNumber = appointment.SerialNumber,
                Problem = appointment.Problem,
                Status = appointment.Status
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(AppointmentVM model)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    AppointmentId = model.AppointmentId,
                    PatientID = model.PatientID,
                    PatientPhoneNumber = model.PatientPhoneNumber,
                    DepartmentId = model.DepartmentId,
                    DoctorId = model.DoctorId,
                    AppoinmentDate = model.AppoinmentDate.Value,
                    SerialNumber = model.SerialNumber,
                    Problem = model.Problem,
                    Status = model.Status
                };

                _unitofWork.AppointmentRepository.Update(appointment);
                TempData["Message"] = "✅ Successfully Updated!";
                TempData["MessageType"] = "primary";

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully Updated!" });
                }

                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid data submitted." });
            }

            var department = _unitofWork.departmentRepo.getAll()
                .Where(c => c.Status == true)
                .ToList();
            var doctor = _unitofWork.doctorRepo.getAll()
                .Where(c => c.Status == true)
                .ToList();

            ViewBag.Department = department;
            ViewBag.Doctor = doctor;

            return View(model);
        }
    }
}
