using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class DesignationController : Controller
    {
        private readonly IUnitOfWork _unitOf;

        public DesignationController(IUnitOfWork unitOf)
        {
            _unitOf = unitOf;
        }

        [Authorize]
        public IActionResult Index()
        {
            
            var designation = _unitOf.designationRepo.getAll().OrderBy(d=>d.DesignationId);
           
            return View(designation);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(Designation designation)
        {
            if (ModelState.IsValid) 
            { 
                _unitOf.designationRepo.Save(designation);

                TempData["Message"] = "✅ Successfully Added!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Save");
            }
           
            return View(designation);  
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var data = _unitOf.designationRepo.Find(Id);
          
            return View(data);
        }

        [HttpPost]
        public IActionResult Update(Designation designation)
        {
            _unitOf.designationRepo.Update(designation);

            TempData["Message"] = "✅ Successfully Added!";
            TempData["MessageType"] = "primary";

            return RedirectToAction("Index");
        }

    
        public IActionResult Delete(int Id)
        {
            _unitOf.designationRepo.Delete(Id);

            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";

            return RedirectToAction("Index");
        }

        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var result = _unitOf.designationRepo.getAll()
                        .Where(p => p.DesignationName?.ToLower().Contains(name) == true)
                        .Select(p => new
                        {
                            p.DesignationId,
                            p.DesignationName,
                            p.Status,
                        }).ToList();

            return Json(result);
        }
    }
}
