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
            ViewBag.Roles = unitofwork.RoleRepository.GetRoles();
            ViewBag.Doctors = unitofwork.doctorRepo.getAll();
            ViewBag.Departments = unitofwork.departmentRepo.getAll().Where(d => d.Status == true).ToList();
            return View(new List<User>());
        }

        [HttpGet]
        public IActionResult GetUsersJson(string search = "", int page = 1, int pageSize = 10)
        {
            var query = unitofwork.UserRepository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(u => (!string.IsNullOrEmpty(u.Name) && u.Name.ToLower().Contains(search)) ||
                                         (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLower().Contains(search)) ||
                                         (!string.IsNullOrEmpty(u.MobileNo) && u.MobileNo.ToLower().Contains(search)));
            }

            query = query.OrderBy(m => m.Id);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    mobileNo = u.MobileNo,
                    userName = u.UserName,
                    status = u.Status
                })
                .ToList();

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
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)  
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "❌ Invalid user data submitted." });
                }
                TempData["Message"] = "❌ Invalid user data submitted.";
                TempData["MessageType"] = "danger";
                ViewBag.Roles = unitofwork.RoleRepository.GetRoles();
                ViewBag.Doctors = unitofwork.doctorRepo.getAll();
                return View(user);
            }

            try
            {
                unitofwork.UserRepository.Save(user);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully added!" });
                }
                TempData["Message"] = "✅ Successfully added!";
                TempData["MessageType"] = "success";
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
                return Json(new { success = true, message = "✅ Successfully Deleted!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "❌ " + (ex?.Message ?? "An error occurred.") });
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
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!ModelState.IsValid)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "❌ Invalid user data submitted." });
                }
                return RedirectToAction("Index");
            }

            try
            {
                unitofwork.UserRepository.Update(user);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully updated!" });
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "❌ " + (ex?.Message ?? "An error occurred.") });
                }
                return RedirectToAction("Index");
            }
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
            try
            {
                unitofwork.UserRepository.GetStatus(id);
                return Json(new { success = true, message = "✅ Status updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "❌ " + (ex?.Message ?? "An error occurred.") });
            }
        }
    }
}
