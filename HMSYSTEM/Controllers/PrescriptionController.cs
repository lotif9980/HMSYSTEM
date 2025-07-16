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

        public IActionResult Index()
        {
           var data=  _unitOfWork.PrescriptioRepository.GetAll();
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
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
            ViewBag.Medicine = medicine;


            var model = new PrescriptionViewModel();
            return View(model);
        }


        [HttpPost]
        public IActionResult Save(PrescriptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // আবার View তে ফিরিয়ে দিতে হবে, ViewBag refill করতে হবে
                ViewBag.Patient = _unitOfWork.PatienRepo.getAll();
                ViewBag.Doctor = _unitOfWork.doctorRepo.getAll();
                ViewBag.Department = _unitOfWork.departmentRepo.getAll();
                ViewBag.Medicine = _unitOfWork.MedicineRepo.GetAllMedicines();
                return View(model);
            }

            // Prescription Entity তৈরি করো ViewModel থেকে
            var prescription = new Prescription
            {
                Date = model.Date ,
                PatientId = model.PatientId,
                DoctorId = model.DoctorId,
                DepartmentId = model.DepartmentId,
                Status = model.Status,
                Note=model.Note,
                NextFlowUp=model.NextFlowUp


            };

            // Child List Map করা
            if (model.PrescriptionDetails != null && model.PrescriptionDetails.Count > 0)
            {
                prescription.PrescriptionDetails = model.PrescriptionDetails.Select(d => new PrescriptionDetail
                {
                    MedicineId = d.MedicineId,
                    Dose = d.Dose,
                    Duration = d.Duration,
                    Instructions = d.Instructions ?? false
                }).ToList();
            }

            // Save prescription (with details) রেপোজিটরিতে
            _unitOfWork.PrescriptioRepository.Save(prescription);
          

            return RedirectToAction("Index"); // অথবা যেই পেজে যেতে চাও
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
            return RedirectToAction("Index");
        }
    }
}
