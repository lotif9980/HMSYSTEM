using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HMSYSTEM.ViewModels;



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
        public IActionResult Index(int page=1, int pageSize=5)
        {
            var totalMedicine=_unitOfWork.MedicineRepo.GetAllMedicines().OrderBy(i=>i.Id);

            var totalItems=totalMedicine.Count();
            var totalPage=(int)Math.Ceiling((double)totalItems / pageSize);

            var medicine=totalMedicine
                         .OrderBy(m=>m.Id)
                         .Skip((page-1)*pageSize)
                         .Take(pageSize)
                         .ToList();

            var studentsViewModel = new PaginationViewModel<Medicine>
            {
                Items = medicine,
                CurrentPage=page,
                PageSize=pageSize,
                TotalPages=totalPage,
                TotalItems = totalItems,

            };
    
            return View(studentsViewModel);
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
