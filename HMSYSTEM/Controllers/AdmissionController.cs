using HMSYSTEM.Helpers;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HMSYSTEM.Controllers
{
    public class AdmissionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdmissionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int page=1 , int pageSize=10)
        {

            var totalAdmission = _unitOfWork.admissionRepository.getAll()
                                   .OrderByDescending(d => d.Id)
                                   .AsQueryable()
                                   .ToPagedList(page, pageSize);
           

            return View(totalAdmission);
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
        public async Task<IActionResult> Save(AdmissionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var exists = await _unitOfWork.admissionRepository.PatientStatusCheck(vm.PatientId);
                if (exists)
                {
                    TempData["Message"] = "✅ Patient Already Added";
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
                return RedirectToAction("Index");
            }

       
            ViewBag.Doctor = _unitOfWork.doctorRepo.getAll();
            ViewBag.Bed = _unitOfWork.bedRepository.getAllBed().Where(p => p.IsOccupied == true).ToList();
            ViewBag.Ward = _unitOfWork.wardRepository.GetAll().ToList();
            int lastSerial = _unitOfWork.admissionRepository.GetLastInvoiceNo();
            ViewBag.NextSerial = lastSerial + 1;

            return View(vm);
        }

        public async Task<IActionResult> GetPatientPhoneNumber(string phoneNumber)
        {
            var patient = _unitOfWork.PatienRepo.getAll()
                .FirstOrDefault(p => p.Phone == phoneNumber && p.Status == true);

            if (patient != null)
            {
               
                var isAdmitted =await _unitOfWork.admissionRepository
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
                .Where(b => b.WardId == wardId && b.IsOccupied==true)
                .Select(b => new { b.Id, b.BedNumber })
                .ToList();

            return Json(beds);
        }

        public IActionResult Delete(int id)
        {
            var admission=_unitOfWork.admissionRepository.GetById(id);

            _unitOfWork.admissionRepository.Delete(id);
            _unitOfWork.bedRepository.StatusUpdate(admission.BedId);
            return RedirectToAction("Index");
        }


        public IActionResult Details(int id)
        {
            var admission=_unitOfWork.admissionRepository.GetById(id);
            return View (admission);
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
                            Id=p.Id,
                            Date=p.AdmitDate,
                            InvoiceNo =p.InvoiceNo,
                            Name=p.Patient.FirstName+" "+ p.Patient.LastName,
                            Number=p.Patient.Phone,
                            WardName=p.Bed?.Ward.Name,
                            BedName=p.Bed.BedNumber,
                            Status=p.Status
                        }).ToList();

            return Json (result);

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
            var doctors=_unitOfWork.doctorRepo.getAll()
                .Where(d=>d.DepartmentId == departmentId)
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
