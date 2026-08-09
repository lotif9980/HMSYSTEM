using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HMSYSTEM.ViewModels;

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
            return View();
        }

        [HttpGet]
        public IActionResult GetDepartments(string search = "", int page = 1, int pageSize = 10)
        {
            var query = _unitOf.departmentRepo.getAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(d => !string.IsNullOrEmpty(d.DepartmentName) && d.DepartmentName.ToLower().Contains(search));
            }

            query = query.OrderBy(d => d.DepartmentId);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(d => new
                {
                    departmentId = d.DepartmentId,
                    departmentName = d.DepartmentName,
                    status = d.Status
                })
                .ToList();

            return Json(new{issuccess = true,totalPages = totalPages,totalItem = totalItem,currentPage = page,data = data});
        }

        [HttpGet]
        public IActionResult GetDepartmentById(int id)
        {
            var department = _unitOf.departmentRepo.Edit(id);
            if (department == null)
            {
                return NotFound();
            }
            return Json(new
            {
                departmentId = department.DepartmentId,
                departmentName = department.DepartmentName,
                status = department.Status
            });
        }

        [HttpPost]
        public IActionResult Save(Department department)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (ModelState.IsValid) 
            {
                department.Status = true;
                _unitOf.departmentRepo.Save(department);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully Added!" });
                }
                TempData["Message"] = "✅ Successfully Added!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
           
            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid Department data submitted." });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            return RedirectToAction("Index", new { editId = Id });
        }

        [HttpPost]
        public IActionResult Update(Department department)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (ModelState.IsValid)
            {
                _unitOf.departmentRepo.Update(department);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully Updated!" });
                }
                TempData["Message"] = "✅ Successfully Updated!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid Department data submitted." });
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int Id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                var data = await _unitOf.departmentRepo.inUsedCheck(Id);
                if (data)
                {
                    if (isAjax)
                    {
                        return Json(new { success = false, message = "❌ Used in Department/Doctor!" });
                    }
                    TempData["Message"] = "❌ Used in Department/Doctor!";
                    TempData["MessageType"] = "danger";
                    return RedirectToAction("Index");
                }
                _unitOf.departmentRepo.Delete(Id);
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
