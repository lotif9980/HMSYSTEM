using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;



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
            return View();
        }

        public IActionResult GetMedicine(string search = "", int page = 1, int pageSize = 10)
       {
            var query = _unitOfWork.MedicineRepo.GetAllMedicines().AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(m => (!string.IsNullOrEmpty(m.Name) && m.Name.ToLower().Contains(search)) || 
                                         (!string.IsNullOrEmpty(m.GenericName) && m.GenericName.ToLower().Contains(search)));
            }
            
            query = query.OrderBy(i => i.Id);
            
            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);
            
            var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
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
            return View();
        }

        [HttpPost]
        public IActionResult Save(Medicine medicine)
        {
            

            if (ModelState.IsValid)
            {
                medicine.IsActive = true;
              _unitOfWork.MedicineRepo.Save(medicine);
              return Json(new { success = true, message = "✅ Successfully Added!" });
            }

            
            return Json(new {success=false,message="Save Faild"});
        }

        [HttpGet]
        public IActionResult GetMedicineById(int id)
        {
            var medicine = _unitOfWork.MedicineRepo.Find(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return Json(new
            {
                id = medicine.Id,
                name = medicine.Name,
                genericName = medicine.GenericName,
                strength = medicine.Strength,
                form = medicine.Form,
                isActive = medicine.IsActive
            });
        }

        [HttpPost]
        public IActionResult Update(Medicine medicine)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                _unitOfWork.MedicineRepo.Update(medicine);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully Updated!" });
                }
                TempData["Message"] = "✅ Successfully Updated!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid medicine data submitted." });
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int Id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                _unitOfWork.MedicineRepo.Delete(Id);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully Deleted!" });
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
