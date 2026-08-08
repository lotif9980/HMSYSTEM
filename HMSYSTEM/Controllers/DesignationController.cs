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
            return View();
        }

        [HttpGet]
        public IActionResult GetDesignations(string search = "", int page = 1, int pageSize = 10)
        {
            var query = _unitOf.designationRepo.getAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(d => !string.IsNullOrEmpty(d.DesignationName) && d.DesignationName.ToLower().Contains(search));
            }

            query = query.OrderBy(d => d.DesignationId);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(d => new
                {
                    designationId = d.DesignationId,
                    designationName = d.DesignationName,
                    status = d.Status
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
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(Designation designation)
        {
           
            if (ModelState.IsValid) 
            {
                designation.Status = true;
                _unitOf.designationRepo.Save(designation);
               return Json(new { success = true, message = "✅ Successfully Added!" });
            }
           
            return Json(new { success = false, message = "❌ Invalid Designation data submitted." });
            
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
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (ModelState.IsValid)
            {
                _unitOf.designationRepo.Update(designation);
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
                return Json(new { success = false, message = "❌ Invalid Designation data submitted." });
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int Id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                _unitOf.designationRepo.Delete(Id);
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

        public IActionResult UpdateStatus(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                _unitOf.designationRepo.StatusUpdate(id);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Status updated successfully!" });
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
    }
}
