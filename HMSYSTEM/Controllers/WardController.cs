using HMSYSTEM.Enum;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class WardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Department = _unitOfWork.departmentRepo.getAll();
            return View();
        }

        public IActionResult GetWard(string search = "", int page = 1, int pageSize = 10)
        {
            var query = _unitOfWork.wardRepository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(w => (!string.IsNullOrEmpty(w.Name) && w.Name.ToLower().Contains(search)) ||
                                         (w.Department != null && !string.IsNullOrEmpty(w.Department.DepartmentName) && w.Department.DepartmentName.ToLower().Contains(search)) ||
                                         (!string.IsNullOrEmpty(w.FloorNo) && w.FloorNo.ToLower().Contains(search)) ||
                                         w.Type.ToString().ToLower().Contains(search));
            }

            query = query.OrderBy(i => i.Id);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize).Select(w => new
            {
                id = w.Id,
                name = w.Name,
                type = w.Type.ToString(),
                typeId = (int)w.Type,
                departmentId = w.DepartmentId,
                departmentName = w.Department != null ? w.Department.DepartmentName : "",
                floorNo = w.FloorNo,
                totalBeds = w.TotalBeds
            }).ToList();

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
            var department = _unitOfWork.departmentRepo.getAll();
            ViewBag.Department = department;
            return View();
        }

        [HttpPost]
        public IActionResult Save(Ward ward)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

           
                _unitOfWork.wardRepository.Save(ward);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully Added!" });
                }
                TempData["Message"] = "✅ Successfully Added";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
           

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Save Failed. Please check inputs." });
            }
            ViewBag.Department = _unitOfWork.departmentRepo.getAll();
            return View(ward);
        }

        [HttpGet]
        public IActionResult GetWardById(int id)
        {
            var ward = _unitOfWork.wardRepository.Find(id);
            if (ward == null)
            {
                return NotFound();
            }
            return Json(new
            {
                id = ward.Id,
                name = ward.Name,
                type = (int)ward.Type,
                typeName = ward.Type.ToString(),
                departmentId = ward.DepartmentId,
                departmentName = ward.Department?.DepartmentName,
                floorNo = ward.FloorNo,
                totalBeds = ward.TotalBeds
            });
        }

        [HttpPost]
        public IActionResult Update(Ward ward)
        {

            if (!string.IsNullOrEmpty(ward.Name) && ward.TotalBeds > 0)
            {
                _unitOfWork.wardRepository.Update(ward);

                return Json(new { success = true, message = "✅ Successfully Updated!" });
                
            }
            
             return Json(new { success = false, message = "❌ Invalid ward data submitted." });
           
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                var isUsed = await _unitOfWork.wardRepository.IsBedinUsed(id);

                if (isUsed)
                {
                    if (isAjax)
                    {
                        return Json(new { success = false, message = "❌ Ward is currently used in Bed(s)!" });
                    }
                    TempData["Message"] = "❌ Ward Used in Bed!";
                    TempData["MessageType"] = "danger";
                    return RedirectToAction("Index");
                }

                _unitOfWork.wardRepository.Delete(id);

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

        public IActionResult Details(int id)
        {
            var data = _unitOfWork.wardRepository.Find(id);
            return View(data);
        }

        [HttpGet]
        public IActionResult GetBedsByWardId(int wardId)
        {
            var beds = _unitOfWork.bedRepository.getAllBed()
                .Where(b => b.WardId == wardId)
                .Select(b => new
                {
                    bedNumber = b.BedNumber,
                    isOccupied = b.IsOccupied
                }).ToList();

            var totalBeds = beds.Count;
            var occupiedBeds = beds.Count(b => b.isOccupied == true);
            var emptyBeds = beds.Count(b => b.isOccupied == false);

            return Json(new
            {
                totalBeds,
                emptyBeds,
                occupiedBeds,
                beds
            });
        }

        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var result = _unitOfWork.wardRepository.GetAll()
                        .Where(p => p.Name?.ToLower().Contains(name) == true)
                        .Select(p => new
                        {
                            p.Name,
                            p.Id,
                            DepartmentName = p.Department != null ? p.Department.DepartmentName : "",
                            p.TotalBeds,
                            p.FloorNo,
                            type = p.Type.ToString(),
                        }).ToList();

            return Json(result);
        }
    }
}
