using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        protected readonly IUnitOfWork unitofwork;

        public UserController(IUnitOfWork unitofwork)
        {
            this.unitofwork = unitofwork;
        }

        [Authorize]
        public IActionResult Index()
        {
            var user = unitofwork.UserRepository.GetAll().OrderBy(m=>m.Id);
            ViewBag.Roles = unitofwork.RoleRepository.GetRoles();
            ViewBag.Doctors = unitofwork.doctorRepo.getAll();
            ViewBag.Departments = unitofwork.departmentRepo.getAll().Where(d => d.Status == true).ToList();
            return View(user);
        }

        [HttpGet]
        public IActionResult GetDoctorsByDepartment(int departmentId)
        {
            var doctors = unitofwork.doctorRepo.getAll()
                .Where(d => d.DepartmentId == departmentId && d.Status == true)
                .Select(d => new
                {
                    id = d.Id,
                    name = d.FirstName + " " + d.LastName
                })
                .ToList();

            return Json(doctors);
        }

        [HttpGet]
        public IActionResult Save()
        {
            ViewBag.Roles=unitofwork.RoleRepository.GetRoles();
            ViewBag.Doctors=unitofwork.doctorRepo.getAll();

            return View();
        }

        [HttpPost]
        public IActionResult Save(User user)
        {
            if (!ModelState.IsValid)  
            {
                TempData["Message"] = "❌ Invalid user data submitted.";
                TempData["MessageType"] = "danger";
                ViewBag.Roles = unitofwork.RoleRepository.GetRoles();
                ViewBag.Doctors = unitofwork.doctorRepo.getAll();
                return View(user);
            }

            try
            {
                unitofwork.UserRepository.Save(user);
                TempData["Message"] = "✅ Successfully added!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "❌ " + (ex?.Message ?? "An error occurred.");
                TempData["MessageType"] = "danger";
                ViewBag.Roles = unitofwork.RoleRepository.GetRoles();
                ViewBag.Doctors = unitofwork.doctorRepo.getAll();
                return View(user);
            }
        }



        public IActionResult Delete(int Id)
        {
            try
            {
                unitofwork.UserRepository.Delete(Id);
                TempData["Message"] = "✅ Successfully Delete!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "❌ " + (ex?.Message ?? "An error occurred.");
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
            
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            return RedirectToAction("Index", new { editId = Id });
        }

        [HttpGet]
        public IActionResult GetUserById(int id)
        {
            var user = unitofwork.UserRepository.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            int departmentId = 0;
            if (user.DoctorId > 0)
            {
                var doctor = unitofwork.doctorRepo.getAll().FirstOrDefault(d => d.Id == user.DoctorId);
                if (doctor != null)
                {
                    departmentId = doctor.DepartmentId ?? 0;
                }
            }

            return Json(new
            {
                id = user.Id,
                name = user.Name,
                mobileNo = user.MobileNo,
                userName = user.UserName,
                roleId = user.RoleId,
                status = user.Status,
                doctorId = user.DoctorId,
                departmentId = departmentId
            });
        }

        [HttpPost]
        public IActionResult Update(User user)
        {

            unitofwork.UserRepository.Update(user);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            var data = unitofwork.UserRepository.Find(Id);

            var roles = unitofwork.RoleRepository.GetRoles();

            ViewBag.Role = roles;
            return View(data);
        }

      
        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var result = unitofwork.UserRepository.GetAll()
                        .Where(p => p.Name?.ToLower().Contains(name) == true)
                        .Select(p => new
                        {
                            Name = p.Name,
                            UserName=p.UserName,
                            Id=p.Id,
                            MobileNo=p.MobileNo,
                            Status=p.Status,

                        }).ToList();

            return Json(result);
        }
       
        public IActionResult GetStatusUpdate(int id)
        {
            unitofwork.UserRepository.GetStatus(id);
           
            return RedirectToAction("Index");
        }
    }
}
