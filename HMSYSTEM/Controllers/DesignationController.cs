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
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
           
            TempData["Message"] = "❌ Invalid Designation data submitted.";
            TempData["MessageType"] = "danger";
            return RedirectToAction("Index");  
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            return RedirectToAction("Index", new { editId = Id });
        }

        [HttpGet]
        public IActionResult GetDesignationById(int id)
        {
            var designation = _unitOf.designationRepo.Find(id);
            if (designation == null)
            {
                return NotFound();
            }
            return Json(new
            {
                designationId = designation.DesignationId,
                designationName = designation.DesignationName,
                status = designation.Status
            });
        }

        [HttpPost]
        public IActionResult Update(Designation designation)
        {
            _unitOf.designationRepo.Update(designation);

            TempData["Message"] = "✅ Successfully Updated!";
            TempData["MessageType"] = "success";

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
