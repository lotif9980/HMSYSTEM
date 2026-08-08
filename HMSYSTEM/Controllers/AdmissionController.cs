using HMSYSTEM.Helpers;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class AdmissionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdmissionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Doctor = _unitOfWork.doctorRepo.getAll();
            ViewBag.Ward = _unitOfWork.wardRepository.GetAll().ToList();
            int lastSerial = _unitOfWork.admissionRepository.GetLastInvoiceNo();
            ViewBag.NextSerial = lastSerial + 1;
            return View();
        }

        public IActionResult GetAdmission(string search = "", int page = 1, int pageSize = 10)
        {
            var query = _unitOfWork.admissionRepository.getAll()
                                   .OrderByDescending(d => d.Id)
                                   .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(a => (a.Patient != null && (
                                            (!string.IsNullOrEmpty(a.Patient.FirstName) && a.Patient.FirstName.ToLower().Contains(search)) ||
                                            (!string.IsNullOrEmpty(a.Patient.LastName) && a.Patient.LastName.ToLower().Contains(search)) ||
                                            ((a.Patient.FirstName + " " + a.Patient.LastName).ToLower().Contains(search)) ||
                                            (!string.IsNullOrEmpty(a.Patient.Phone) && a.Patient.Phone.ToLower().Contains(search))
                                         )) ||
                                         a.InvoiceNo.ToString().Contains(search) ||
                                         (a.Bed != null && (
                                            (!string.IsNullOrEmpty(a.Bed.BedNumber) && a.Bed.BedNumber.ToLower().Contains(search)) ||
                                            (a.Bed.Ward != null && !string.IsNullOrEmpty(a.Bed.Ward.Name) && a.Bed.Ward.Name.ToLower().Contains(search))
                                         )) ||
                                         (a.Doctor != null && (
                                            (!string.IsNullOrEmpty(a.Doctor.FirstName) && a.Doctor.FirstName.ToLower().Contains(search)) ||
                                            (!string.IsNullOrEmpty(a.Doctor.LastName) && a.Doctor.LastName.ToLower().Contains(search))
                                         )));
            }

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize).Select(a => new
            {
                id = a.Id,
                admitDate = a.AdmitDate.HasValue ? a.AdmitDate.Value.ToString("dd-MM-yyyy") : "",
                invoiceNo = a.InvoiceNo,
                patientId = a.PatientId,
                patientName = a.Patient != null ? (a.Patient.FirstName + " " + a.Patient.LastName).Trim() : "",
                patientPhone = a.Patient != null ? a.Patient.Phone : "",
                doctorId = a.DoctorId,
                doctorName = a.Doctor != null ? (a.Doctor.FirstName + " " + a.Doctor.LastName).Trim() : "",
                bedId = a.BedId,
                bedNumber = a.Bed != null ? a.Bed.BedNumber : "",
                wardName = a.Bed != null && a.Bed.Ward != null ? a.Bed.Ward.Name : "",
                status = a.Status,
                attendentName = a.AttendentName,
                attendentRelation = a.AttendentRelation,
                attendentPhone = a.AttendentPhone,
                forReason = a.ForReason,
                declaration = a.Declaration
            }).ToList();

            return Json(new
            {
                issuccess = true,
                totalPages = totalPages,
                totalItem = totalItem,
                currentPage = page,
                data = data
            });
        }

        [HttpGet]
        public IActionResult Save()
        {
            var doctors = _unitOfWork.doctorRepo.getAll();
            ViewBag.Doctor = doctors;

            var beds = _unitOfWork.bedRepository.getAllBed().ToList().Where(p => p.IsOccupied == true);
            ViewBag.Bed = beds;

            var wards = _unitOfWork.wardRepository.GetAll().ToList();
            ViewBag.Ward = wards;

            int lastSerial = _unitOfWork.admissionRepository.GetLastInvoiceNo();
            int nextSerial = lastSerial + 1;
            ViewBag.NextSerial = nextSerial;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(AdmissionViewModel vm)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                var exists = await _unitOfWork.admissionRepository.PatientStatusCheck(vm.PatientId);
                if (exists)
                {
                    if (isAjax)
                    {
                        return Json(new { success = false, message = "❌ Patient Already Admitted!" });
                    }
                    TempData["Message"] = "❌ Patient Already Added";
                    TempData["MessageType"] = "danger";
                    return View(vm);
                }

                _unitOfWork.bedRepository.StatusUpdate(vm.BedId);

                var admission = new Admission
                {
                    Id = vm.Id,
                    PatientId = vm.PatientId,
                    DoctorId = vm.DoctorId,
                    BedId = vm.BedId,
                    AdmitDate = vm.AdmitDate,
                    Status = vm.Status,
                    InvoiceNo = vm.InvoiceNo,
                    AttendentName = vm.AttendentName,
                    AttendentRelation = vm.AttendentRelation,
                    AttendentPhone = vm.AttendentPhone,
                    ForReason = vm.ForReason,
                    Declaration = vm.Declaration
                };

                _unitOfWork.admissionRepository.Save(admission);

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Admission Successfully Created!" });
                }
                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid admission data submitted." });
            }

            ViewBag.Doctor = _unitOfWork.doctorRepo.getAll();
            ViewBag.Bed = _unitOfWork.bedRepository.getAllBed().Where(p => p.IsOccupied == true).ToList();
            ViewBag.Ward = _unitOfWork.wardRepository.GetAll().ToList();
            int lastSerial = _unitOfWork.admissionRepository.GetLastInvoiceNo();
            ViewBag.NextSerial = lastSerial + 1;

            return View(vm);
        }

        [HttpGet]
        public IActionResult GetAdmissionById(int id)
        {
            var admission = _unitOfWork.admissionRepository.GetById(id);
            if (admission == null)
            {
                return NotFound();
            }
            return Json(new
            {
                id = admission.Id,
                invoiceNo = admission.InvoiceNo,
                admitDate = admission.AdmitDate.HasValue ? admission.AdmitDate.Value.ToString("yyyy-MM-dd") : "",
                patientId = admission.PatientId,
                patientName = admission.Patient != null ? (admission.Patient.FirstName + " " + admission.Patient.LastName).Trim() : "",
                doctorId = admission.DoctorId,
                bedId = admission.BedId,
                attendentName = admission.AttendentName,
                attendentRelation = admission.AttendentRelation,
                attendentPhone = admission.AttendentPhone,
                forReason = admission.ForReason,
                declaration = admission.Declaration,
                status = admission.Status
            });
        }

        [HttpPost]
        public IActionResult Update(Admission admission)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                _unitOfWork.admissionRepository.Update(admission);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Admission Successfully Updated!" });
                }
                TempData["Message"] = "✅ Successfully Updated!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid admission data submitted." });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                var admission = _unitOfWork.admissionRepository.GetById(id);
                if (admission == null)
                {
                    if (isAjax) return Json(new { success = false, message = "❌ Admission record not found!" });
                    TempData["Message"] = "❌ Record not found!";
                    TempData["MessageType"] = "danger";
                    return RedirectToAction("Index");
                }

                _unitOfWork.admissionRepository.Delete(id);
                _unitOfWork.bedRepository.StatusUpdate(admission.BedId);

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Admission Successfully Deleted!" });
                }
                TempData["Message"] = "✅ Successfully Deleted!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "❌ " + (ex?.Message ?? "An error occurred.") });
                }
                TempData["Message"] = "❌ " + (ex?.Message ?? "An error occurred.");
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            _unitOfWork.admissionRepository.UpdateAdmissionStatus(id);
            if (isAjax)
            {
                return Json(new { success = true, message = "✅ Admission status updated!" });
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetPatientPhoneNumber(string phoneNumber)
        {
            var patient = _unitOfWork.PatienRepo.getAll()
                .FirstOrDefault(p => p.Phone == phoneNumber && p.Status == true);

            if (patient != null)
            {
                var isAdmitted = await _unitOfWork.admissionRepository
                    .PatientStatusCheck(patient.PatientID);

                if (isAdmitted)
                {
                    return Json(new
                    {
                        success = false,
                        alreadyAdded = true,
                        message = "✅ This patient is already admitted."
                    });
                }

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
                return Json(new
                {
                    success = false,
                    alreadyAdded = false,
                    message = "❌ Patient not found with this number."
                });
            }
        }

        [HttpGet]
        public IActionResult GetBedsByWardId(int wardId)
        {
            var beds = _unitOfWork.bedRepository.getAllBed()
                .Where(b => b.WardId == wardId && b.IsOccupied == false)
                .Select(b => new { b.Id, b.BedNumber })
                .ToList();

            return Json(beds);
        }

        public IActionResult Details(int id)
        {
            var admission = _unitOfWork.admissionRepository.GetById(id);
            return View(admission);
        }

        public IActionResult GetPrintPartial(int id)
        {
            var data = _unitOfWork.admissionRepository.GetById(id);
            return PartialView("_PartialPrintAdmission", data);
        }

        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var result = _unitOfWork.admissionRepository.getAll()
                        .Where(d =>
                            ((d.Patient.FirstName ?? "").Trim().ToLower().Contains(name)) ||
                            ((d.Patient.LastName ?? "").Trim().ToLower().Contains(name)) ||
                            (((d.Patient.FirstName ?? "") + " " + (d.Patient.LastName ?? "")).Trim().ToLower().Contains(name))
                        ).Select(p => new
                        {
                            Id = p.Id,
                            Date = p.AdmitDate,
                            InvoiceNo = p.InvoiceNo,
                            Name = p.Patient.FirstName + " " + p.Patient.LastName,
                            Number = p.Patient.Phone,
                            WardName = p.Bed?.Ward.Name,
                            BedName = p.Bed.BedNumber,
                            Status = p.Status
                        }).ToList();

            return Json(result);
        }

        #region saveDetails Describe

        [HttpGet]
        public IActionResult SaveTow()
        {
            var doctors = _unitOfWork.doctorRepo.getAll();
            ViewBag.Doctor = doctors;

            var beds = _unitOfWork.bedRepository.getAllBed().ToList().Where(p => p.IsOccupied == true);
            ViewBag.Bed = beds;

            var wards = _unitOfWork.wardRepository.GetAll().ToList();
            ViewBag.Ward = wards;

            int lastSerial = _unitOfWork.admissionRepository.GetLastInvoiceNo();
            int nextSerial = lastSerial + 1;
            ViewBag.NextSerial = nextSerial;

            var department = _unitOfWork.departmentRepo.getAll();
            ViewBag.Department = department;

            return View();
        }

        public IActionResult GetDoctrobyDepartment(int departmentId)
        {
            var doctors = _unitOfWork.doctorRepo.getAll()
                .Where(d => d.DepartmentId == departmentId)
                .Select(d => new
                {
                    d.FirstName,
                    d.Id
                })
                .ToList();

            return Json(doctors);
        }

        #endregion
    }
}
