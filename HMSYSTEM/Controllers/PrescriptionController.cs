using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HMSYSTEM.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PrescriptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index( int pageSize=10 , int page=1)
        {
            var totalPrescription=  _unitOfWork.PrescriptioRepository.GetAll().OrderByDescending(p=>p.Id);
            var totalItem= totalPrescription.Count();
            var totalPage=(int)Math.Ceiling((decimal)totalItem/pageSize);
            var prescription = totalPrescription
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            var viewModel = new PaginationViewModel<Prescription>
            {
                Items= prescription,
                TotalItems= totalItem,
                PageSize= pageSize,
                TotalPages= totalPage,
                CurrentPage=page
            };
            return View(viewModel);
        }


        #region old save method
        [HttpGet]
        //public IActionResult Save()
        //{
        //    var department = _unitOfWork.departmentRepo.getAll()
        //        .Where(c => c.Status == true)
        //        .ToList();
        //    var doctor = _unitOfWork.doctorRepo.getAll().
        //        Where(c => c.Status == true)
        //        .ToList();
        //    var patient = _unitOfWork.PatienRepo.getAll().ToList();

        //    var medicine = _unitOfWork.MedicineRepo.GetAllMedicines().ToList();

        //    ViewBag.Department = department;
        //    ViewBag.Doctor = doctor;
        //    ViewBag.Patient = patient;
        //    ViewBag.Medicine = medicine;


        //    var model = new PrescriptionViewModel();
        //    return View(model);
        //}

        #endregion


        [HttpGet]
        public IActionResult Save(int appointmentId)
        {
            // Appointment fetch
            var appointment = _unitOfWork.AppointmentRepository.GetProgress()
                .FirstOrDefault(a => a.AppointmentId == appointmentId);

            if (appointment == null)
                return NotFound();

            // Prescription ViewModel fill
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

            // Medicine list for dropdown
            var medicineList = _unitOfWork.MedicineRepo.GetAllMedicines().ToList();
            ViewBag.Medicine = medicineList;

            return View(model);
        }
        #region
        //[HttpPost]
        //public IActionResult Save(PrescriptionViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {

        //        ViewBag.Patient = _unitOfWork.PatienRepo.getAll();
        //        ViewBag.Doctor = _unitOfWork.doctorRepo.getAll();
        //        ViewBag.Department = _unitOfWork.departmentRepo.getAll();
        //        ViewBag.Medicine = _unitOfWork.MedicineRepo.GetAllMedicines();
        //        return View(model);
        //    }


        //    var prescription = new Prescription
        //    {
        //        Date = model.Date ,
        //        PatientId = model.PatientId,
        //        DoctorId = model.DoctorId,
        //        DepartmentId = model.DepartmentId,
        //        Status = model.Status,
        //        Note=model.Note,
        //        NextFlowUp=model.NextFlowUp
        //    };


        //    if (model.PrescriptionDetails != null && model.PrescriptionDetails.Count > 0)
        //    {
        //        prescription.PrescriptionDetails = model.PrescriptionDetails.Select(d => new PrescriptionDetail
        //        {
        //            MedicineId = d.MedicineId,
        //            Dose = d.Dose,
        //            Duration = d.Duration,
        //            Instructions = d.Instructions ?? false
        //        }).ToList();
        //    }


        //    _unitOfWork.PrescriptioRepository.Save(prescription);

        //    TempData["Message"] = "✅ Successfully added!";
        //    TempData["MessageType"] = "success";


        //    return RedirectToAction("Index"); 
        //}
        #endregion

        [HttpPost]
        public IActionResult Save(PrescriptionViewModel model)
        {
            // Model validation check
            if (!ModelState.IsValid)
            {
                // Preserve dropdown / readonly data for re-display
                ViewBag.Patient = _unitOfWork.PatienRepo.getAll();
                ViewBag.Doctor = _unitOfWork.doctorRepo.getAll();
                ViewBag.Department = _unitOfWork.departmentRepo.getAll();
                ViewBag.Medicine = _unitOfWork.MedicineRepo.GetAllMedicines();

                return View(model);
            }

            // Create Prescription master record
            var prescription = new Prescription
            {
                AppointmentId=model.AppointmentId,
                Date = model.Date ?? DateTime.Now,
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                DepartmentId = model.DepartmentId,
                Status = model.Status,
                Note = model.Note,
                NextFlowUp = model.NextFlowUp
            };

            // Map PrescriptionDetails if any
            if (model.PrescriptionDetails != null && model.PrescriptionDetails.Count > 0)
            {
                prescription.PrescriptionDetails = model.PrescriptionDetails
                    .Where(d => d.MedicineId.HasValue) // ignore empty rows
                    .Select(d => new PrescriptionDetail
                    {
                        MedicineId = d.MedicineId.Value,
                        Dose = d.Dose,
                        Duration = d.Duration,
                        Instructions = d.Instructions ?? false,
                    }).ToList();
            }

            // Save using repository
            _unitOfWork.PrescriptioRepository.Save(prescription);

            TempData["Message"] = "✅ Successfully added!";
            TempData["MessageType"] = "success";

            _unitOfWork.AppointmentRepository.UpdateStatus(model.AppointmentId, Enum.AppointmentStatus.Completed);

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
            _unitOfWork.PrescriptioRepository.Delete(id);
            TempData["Message"] = "✅ Successfully Delete!";
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
                    Id=p.Id,
                    Date=p.Date,
                    PatientName=p.Patient.FirstName +" "+p.Patient.LastName,
                    DoctorName=p.Doctor.FirstName + " "+ p.Doctor.LastName,
                    DepartmentName=p.Department.DepartmentName,

                }).ToList();


            return Json(result);
        }
    }
}
