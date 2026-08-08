using HMSYSTEM.Enum;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static HMSYSTEM.Helpers.QueryableExtensions;

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
            ViewBag.Doctors = _unitofWork.doctorRepo.getAll().Where(c => c.Status == true).ToList();
            ViewBag.Department = _unitofWork.departmentRepo.getAll().Where(c => c.Status == true).ToList();
            return View();
        }

        [HttpGet]
        public IActionResult GetSchedules(string search = "", int page = 1, int pageSize = 10)
        {
            int roleId = Helper.GetRoleId(User);
            int doctorId = Helper.GetDoctorId(User);

            IQueryable<Schedule> query;

            if (roleId == (int)RoleEnum.Doctor && doctorId > 0)
            {
                query = _unitofWork.scheduleRepo.getAll(doctorId).OrderBy(d => d.ScheduleId);
            }
            else
            {
                query = _unitofWork.scheduleRepo.getAll().OrderBy(d => d.ScheduleId);
            }

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(s =>
                    (s.Doctor != null && (
                        (!string.IsNullOrEmpty(s.Doctor.FirstName) && s.Doctor.FirstName.ToLower().Contains(search)) ||
                        (!string.IsNullOrEmpty(s.Doctor.LastName) && s.Doctor.LastName.ToLower().Contains(search))
                    )) ||
                    (s.Department != null && !string.IsNullOrEmpty(s.Department.DepartmentName) && s.Department.DepartmentName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(s.DayOfWeek) && s.DayOfWeek.ToLower().Contains(search))
                );
            }

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(s => new
                {
                    scheduleId = s.ScheduleId,
                    date = s.Date != null ? s.Date.Value.ToString("dd-MM-yyyy") : "N/A",
                    doctorName = (s.Doctor != null ? (s.Doctor.FirstName + " " + s.Doctor.LastName) : "N/A"),
                    departmentName = s.Department != null ? s.Department.DepartmentName : "N/A",
                    dayOfWeek = s.DayOfWeek,
                    startTime = s.StartTime.HasValue ? DateTime.Today.Add(s.StartTime.Value).ToString("hh:mm tt") : "N/A",
                    endTime = s.EndTime.HasValue ? DateTime.Today.Add(s.EndTime.Value).ToString("hh:mm tt") : "N/A",
                    isAvailable = s.IsAvailable
                })
                .ToList();

            return Json(new
            {
                issuccess = true,
                totalPages,
                totalItem,
                currentPage = page,
                data
            });
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
            if (ModelState.IsValid)
            { 
                _unitofWork.scheduleRepo.Save(schedule);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Schedule added successfully!" });
                }

                TempData["Message"] = "✅ Successfully Added!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Save");
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Validation failed. Please check inputs." });
            }

            return View(schedule);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var data = _unitofWork.scheduleRepo.Edit(Id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (data == null) return Json(new { success = false, message = "Schedule not found" });

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        scheduleId = data.ScheduleId,
                        date = data.Date.HasValue ? data.Date.Value.ToString("yyyy-MM-dd") : "",
                        doctorId = data.DoctorId,
                        departmentId = data.DepartmentId,
                        dayOfWeek = data.DayOfWeek,
                        startTime = data.StartTime.HasValue ? DateTime.Today.Add(data.StartTime.Value).ToString("HH:mm") : "",
                        endTime = data.EndTime.HasValue ? DateTime.Today.Add(data.EndTime.Value).ToString("HH:mm") : "",
                        isAvailable = data.IsAvailable ?? true
                    }
                });
            }

            ViewBag.Doctors = _unitofWork.doctorRepo.getAll().Where(c => c.Status == true);
            ViewBag.Department = _unitofWork.departmentRepo.getAll().Where(c => c.Status == true);
            return View(data);
        }

        [HttpPost]
        public IActionResult Update(Schedule schedule)
        {
            var data = _unitofWork.scheduleRepo.Update(schedule);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, message = "Schedule updated successfully!" });
            }

            TempData["Message"] = "✅ Successfully Update!";
            TempData["MessageType"] = "primary";
            return RedirectToAction("Index");
        }

    
        public IActionResult Delete(int Id)
        {
            _unitofWork.scheduleRepo.Delete(Id);
            TempData["Message"] = "❌ Successfully Delete!";
            TempData["MessageType"] = "danger";


            return RedirectToAction("Index");
        }

        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";
            var result = _unitofWork.scheduleRepo.getAll()
                 .Where(p =>
                    ((p.Doctor.FirstName ?? "").Trim().ToLower().Contains(name)) ||
                    ((p.Doctor.LastName ?? "").Trim().ToLower().Contains(name)) ||
                    (((p.Doctor.FirstName ?? "") + " " + (p.Doctor.LastName ?? "")).Trim().ToLower().Contains(name))
                  ).Select(p => new
                  {
                   name=p.Doctor.FirstName +" "+p.Doctor.LastName,
                  
                    Date=p.Date,
                    DepartmentName=p.Department.DepartmentName,
                    DayOfWeek=p.DayOfWeek,
                    Id=p.ScheduleId,
                    StartTime=p.StartTime,
                }).ToList();

            return Json(result);
        }
        public IActionResult GetDoctorsByDepartment(int departmentId)
        {
            var doctors = _unitofWork.doctorRepo.getAll()
                .Where(d => d.DepartmentId == departmentId && d.Status == true)
                .Select(d => new { id = d.Id, name = d.FirstName + " " + d.LastName })
                .ToList();
            return Json(doctors);
        }
    }
}
