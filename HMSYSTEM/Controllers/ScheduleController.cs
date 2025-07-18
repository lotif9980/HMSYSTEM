using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        protected readonly IUnitOfWork _unitofWork;

        public ScheduleController(IUnitOfWork unitOfWork)
        {
            this._unitofWork = unitOfWork;

        }

        [Authorize]
        public IActionResult Index()
        {

            var data = _unitofWork.scheduleRepo.getAll();
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
        {
            ViewBag.Doctors = _unitofWork.doctorRepo.getAll().Where(c=>c.Status==true);
            ViewBag.Department = _unitofWork.departmentRepo.getAll().Where(c => c.Status == true);
            return View();
        }

        [HttpPost]
        public IActionResult Save(Schedule schedule)
        {
            _unitofWork.scheduleRepo.Save(schedule);

            return RedirectToAction("Save");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var data = _unitofWork.scheduleRepo.Edit(Id);

            ViewBag.Doctors = _unitofWork.doctorRepo.getAll().Where(c => c.Status == true);
            ViewBag.Department = _unitofWork.departmentRepo.getAll().Where(c => c.Status == true);
            TempData["Message"] = "✅ Successfully Added!";
            TempData["MessageType"] = "primary";


            return View(data);
        }

        [HttpPost]
        public IActionResult Update(Schedule schedule)
        {
            var data = _unitofWork.scheduleRepo.Update(schedule);
            return RedirectToAction("Index", data);
        }

    
        public IActionResult Delete(int Id)
        {
            _unitofWork.scheduleRepo.Delete(Id);
            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";


            return RedirectToAction("Index");
        }
    }
}
