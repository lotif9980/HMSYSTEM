using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;



namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class MedicineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public MedicineController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var data =_unitOfWork.MedicineRepo.GetAllMedicines();
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(Medicine medicine)
        {
            _unitOfWork.MedicineRepo.Save(medicine);

            TempData["Message"] = "✅ Successfully Added!";
            TempData["MessageType"] = "primary";
                
            return RedirectToAction("Save");
        }

        public IActionResult Delete(int Id)
        {
            _unitOfWork.MedicineRepo.Delete(Id);
            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";
            return RedirectToAction("Index");
        }

        public IActionResult Search(string name)
        {

            name = name?.Trim().ToLower() ?? "";

            var result = _unitOfWork.MedicineRepo.GetAllMedicines()
                .Where(m => !string.IsNullOrEmpty(m.Name) && m.Name.ToLower().Contains(name))
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Strength,
                    m.GenericName,
                    m.Form
                }).ToList();

            return Json(result);
        }
    }
}
