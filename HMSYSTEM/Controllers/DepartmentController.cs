using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        protected readonly IUnitOfWork _unitOf;

        public DepartmentController(IUnitOfWork unitOf)
        {
            _unitOf = unitOf;
        }


        [HttpGet]
        [Authorize]
        
        public IActionResult Index()
        {

            var data = _unitOf.departmentRepo.getAll();
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(Department department)
        {
            _unitOf.departmentRepo.Save(department);
            TempData["Message"] = "✅ Successfully Added!";
            TempData["MessageType"] = "primary";
            return RedirectToAction("Save");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var data = _unitOf.departmentRepo.Edit(Id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Update(Department department)
        {
           var data=  _unitOf.departmentRepo.Update(department);
            TempData["Message"] = "✅ Successfully Added!";
            TempData["MessageType"] = "primary";

            return RedirectToAction("Save",data);
        }
        public async Task<IActionResult> Delete(int Id)
        {
            var data =await _unitOf.departmentRepo.inUsedCheck(Id);
            if (data)
            {
                TempData["Message"] = "✅ Used in Department!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
            _unitOf.departmentRepo.Delete(Id);
            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";
            return RedirectToAction("Index");
        }

        public IActionResult GetNameSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var data = _unitOf.departmentRepo.getAll()
                .Where(m => !string.IsNullOrEmpty(m.DepartmentName)&& m.DepartmentName.ToLower().Contains(name))
                .Select(m => new
                {
                    m.DepartmentName,
                    m.DepartmentId,
                    m.Status
                }).ToList();


            return Json(data);
        }

    }
}
