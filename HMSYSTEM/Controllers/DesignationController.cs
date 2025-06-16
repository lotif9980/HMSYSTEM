using HMSYSTEM.Models;
using HMSYSTEM.Repository;
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
            var data = _unitOf.designationRepo.getAll();
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(Designation designation)
        {
            _unitOf.designationRepo.Save(designation);
            return RedirectToAction("Save");
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
            return RedirectToAction("Index");
        }

    
        public IActionResult Delete(int Id)
        {
            _unitOf.designationRepo.Delete(Id);
            return RedirectToAction("Index");
        }
    }
}
