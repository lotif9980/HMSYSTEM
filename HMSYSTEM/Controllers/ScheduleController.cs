using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
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
        public IActionResult Index( int page=1, int pageSize=10)
        {

            var totalSchedule = _unitofWork.scheduleRepo.getAll().OrderBy(d=>d.ScheduleId);
            var totalItem=totalSchedule.Count();
            var totalPage = (int)Math.Ceiling((double)totalItem / pageSize);

            var schedule=totalSchedule
                        .Skip((page-1)*pageSize)
                        .Take(pageSize)
                        .ToList();

            var viewModel = new PaginationViewModel<Schedule>
            {
                Items = schedule,
                TotalItems = totalItem,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = totalPage
            };
            return View(viewModel);
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

        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";
            var result = _unitofWork.scheduleRepo.getAll()
                .Where(p=>p.Doctor !=null &&
                    (! string.IsNullOrEmpty(p.Doctor.FirstName) && p.Doctor.FirstName.ToLower().Contains(name)) ||
                    (! string.IsNullOrEmpty(p.Doctor.LastName) && p.Doctor.LastName.ToLower().Contains(name))

                ).Select(p => new
                {
                    FirstName=p.Doctor.FirstName,
                    LastName=p.Doctor.LastName,
                    Date=p.Date,
                    DepartmentName=p.Department.DepartmentName,
                    DayOfWeek=p.DayOfWeek,
                    Id=p.ScheduleId,
                    StartTime=p.StartTime,

                }).ToList();

            return Json(result);
        }
    }
}
