using HMSYSTEM.Enum;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static HMSYSTEM.Helpers.QueryableExtensions;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class PrescriptionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PrescriptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var medicineList = _unitOfWork.MedicineRepo.GetAllMedicines().ToList();
            var patientList = _unitOfWork.PatienRepo.getAll().ToList();
            var doctorList = _unitOfWork.doctorRepo.getAll().Where(d => d.Status == true).ToList();
            var departmentList = _unitOfWork.departmentRepo.getAll().Where(d => d.Status == true).ToList();

            ViewBag.Medicine = medicineList;
            ViewBag.Patient = patientList;
            ViewBag.Doctor = doctorList;
            ViewBag.Department = departmentList;

            return View();
        }

        [HttpGet]
        public IActionResult GetPrescriptions(string search = "", int page = 1, int pageSize = 10)
        {
            int roleId = Helper.GetRoleId(User);
            int doctorId = Helper.GetDoctorId(User);

            IQueryable<Prescription> query;

            if (roleId == (int)RoleEnum.Doctor && doctorId > 0)
            {
                query = _unitOfWork.PrescriptioRepository.GetAll(doctorId);
            }
            else
            {
                query = _unitOfWork.PrescriptioRepository.GetAll();
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(p =>
                    (p.Patient != null && (
                        (!string.IsNullOrEmpty(p.Patient.FirstName) && p.Patient.FirstName.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(p.Patient.LastName) && p.Patient.LastName.ToLower().Contains(search))
                    )) ||
                    (p.Doctor != null && (
                        (!string.IsNullOrEmpty(p.Doctor.FirstName) && p.Doctor.FirstName.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(p.Doctor.LastName) && p.Doctor.LastName.ToLower().Contains(search))
                    )) ||
                    (p.Department != null && !string.IsNullOrEmpty(p.Department.DepartmentName) && p.Department.DepartmentName.ToLower().Contains(search))
                );
            }

            query = query.OrderByDescending(p => p.Id);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(p => new
                {
                    id = p.Id,
                    date = p.Date != null ? p.Date.Value.ToString("dd-MM-yyyy") : "N/A",
                    patientName = p.Patient != null ? (p.Patient.FirstName + " " + p.Patient.LastName) : "N/A",
                    doctorName = p.Doctor != null ? (p.Doctor.FirstName + " " + p.Doctor.LastName) : "N/A",
                    departmentName = p.Department != null ? p.Department.DepartmentName : "N/A",
                    status = p.Status
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

        [HttpGet]
        public IActionResult GetSaveModalData(int? appointmentId)
        {
            var medicines = _unitOfWork.MedicineRepo.GetAllMedicines()
                .Select(m => new { id = m.Id, name = m.Name })
                .ToList();

            var patients = _unitOfWork.PatienRepo.getAll()
                .Select(p => new { id = p.PatientID, name = (p.FirstName + " " + p.LastName).Trim(), phone = p.Phone })
                .ToList();

            var doctors = _unitOfWork.doctorRepo.getAll()
                .Select(d => new { id = d.Id, name = (d.FirstName + " " + d.LastName).Trim() })
                .ToList();

            var departments = _unitOfWork.departmentRepo.getAll()
                .Select(d => new { id = d.DepartmentId, name = d.DepartmentName })
                .ToList();

            object appointmentData = null;
            if (appointmentId.HasValue && appointmentId.Value > 0)
            {
                var app = _unitOfWork.AppointmentRepository.GetProgress()
                    .FirstOrDefault(a => a.AppointmentId == appointmentId.Value);

                if (app != null)
                {
                    appointmentData = new
                    {
                        appointmentId = app.AppointmentId,
                        patientId = app.PatientID,
                        patientName = (app.Patient?.FirstName + " " + app.Patient?.LastName).Trim(),
                        patientPhone = app.Patient?.Phone,
                        doctorId = app.DoctorId,
                        doctorName = (app.Doctor?.FirstName + " " + app.Doctor?.LastName).Trim(),
                        departmentId = app.DepartmentId,
                        departmentName = app.Department?.DepartmentName
                    };
                }
            }

            return Json(new
            {
                success = true,
                medicines = medicines,
                patients = patients,
                doctors = doctors,
                departments = departments,
                appointment = appointmentData
            });
        }

        [HttpGet]
        public IActionResult Save(int appointmentId)
        {
            var appointment = _unitOfWork.AppointmentRepository.GetProgress()
                .FirstOrDefault(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                return NotFound();

            var model = new PrescriptionViewModel
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientID,
                PatientName = appointment.Patient.FirstName + " " + appointment.Patient.LastName,
                PatientMobileNo = appointment.Patient.Phone,
                DepartmentId = appointment.DepartmentId ?? 0,
                DepartmentName = appointment.Department?.DepartmentName,
                DoctorId = appointment.DoctorId ?? 0,
                DoctorName = appointment.Doctor?.FirstName + " " + appointment.Doctor?.LastName,
                DesignationId = appointment.Doctor?.DesignationId ?? 0,
                DesignationName = appointment.Doctor?.Designation?.DesignationName,
                Date = DateTime.Now
            };

            var medicineList = _unitOfWork.MedicineRepo.GetAllMedicines().ToList();
            ViewBag.Medicine = medicineList;

            return View(model);
        }

        [HttpPost]
        public IActionResult Save(PrescriptionViewModel model)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "❌ Please complete all required prescription fields." });
                }
                ViewBag.Patient = _unitOfWork.PatienRepo.getAll();
                ViewBag.Doctor = _unitOfWork.doctorRepo.getAll();
                ViewBag.Department = _unitOfWork.departmentRepo.getAll();
                ViewBag.Medicine = _unitOfWork.MedicineRepo.GetAllMedicines();

                return View(model);
            }

            var prescription = new Prescription
            {
                AppointmentId = model.AppointmentId,
                Date = model.Date ?? DateTime.Now,
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                DepartmentId = model.DepartmentId,
                Status = model.Status,
                Note = model.Note,
                NextFlowUp = model.NextFlowUp
            };

            if (model.PrescriptionDetails != null && model.PrescriptionDetails.Count > 0)
            {
                prescription.PrescriptionDetails = model.PrescriptionDetails
                    .Where(d => d.MedicineId.HasValue)
                    .Select(d => new PrescriptionDetail
                    {
                        MedicineId = d.MedicineId.Value,
                        Dose = d.Dose,
                        Duration = d.Duration,
                        Instructions = d.Instructions ?? false,
                    }).ToList();
            }

            _unitOfWork.PrescriptioRepository.Save(prescription);

            if (model.AppointmentId > 0)
            {
                _unitOfWork.AppointmentRepository.UpdateStatus(model.AppointmentId, Enum.AppointmentStatus.Completed);
            }

            if (isAjax)
            {
                return Json(new { success = true, message = "✅ Prescription Successfully Saved & Issued!" });
            }

            TempData["Message"] = "✅ Successfully added!";
            TempData["MessageType"] = "success";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create(PrescriptionViewModel model)
        {
            var department = _unitOfWork.departmentRepo.getAll()
                .Where(c => c.Status == true)
                .ToList();
            var doctor = _unitOfWork.doctorRepo.getAll().
                Where(c => c.Status == true)
                .ToList();
            var patient = _unitOfWork.PatienRepo.getAll().ToList();
            var medicine = _unitOfWork.MedicineRepo.GetAllMedicines().ToList();

            ViewBag.Department = department;
            ViewBag.Doctor = doctor;
            ViewBag.Patient = patient;
            ViewBag.Medicine = medicine
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                })
                .ToList();
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var data = _unitOfWork.PrescriptioRepository.Find(id);
            if (data != null && data.AppointmentId > 0)
            {
                _unitOfWork.AppointmentRepository.UpdateStatus(data.AppointmentId, Enum.AppointmentStatus.InProgress);
            }

            _unitOfWork.PrescriptioRepository.Delete(id);
            TempData["Message"] = "✅ Successfully Deleted!";
            TempData["MessageType"] = "danger";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var vm = _unitOfWork.PrescriptioRepository.GetPrescriptionViewModel(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        public IActionResult GetPrescriptionPrintPartial(int id)
        {
            var doctor = _unitOfWork.PrescriptioRepository.GetPrescriptionViewModel(id);
            if (doctor == null) return NotFound();
            return PartialView("_PrescriptionPrintPartial", doctor);
        }

        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var result = _unitOfWork.PrescriptioRepository.GetAll()
                 .Where(p =>
                    ((p.Patient.FirstName ?? "").Trim().ToLower().Contains(name)) ||
                    ((p.Patient.LastName ?? "").Trim().ToLower().Contains(name)) ||
                    (((p.Patient.FirstName ?? "") + " " + (p.Patient.LastName ?? "")).Trim().ToLower().Contains(name))
                  )
                .Select(p => new
                {
                    Id = p.Id,
                    Date = p.Date,
                    PatientName = p.Patient.FirstName + " " + p.Patient.LastName,
                    DoctorName = p.Doctor.FirstName + " " + p.Doctor.LastName,
                    DepartmentName = p.Department.DepartmentName,
                }).ToList();

            return Json(result);
        }
    }
}
