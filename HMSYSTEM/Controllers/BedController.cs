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
    public class BedController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BedController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Ward = _unitOfWork.wardRepository.GetAll();
            return View();
        }

        public IActionResult GetBed(string search = "", int page = 1, int pageSize = 10)
        {
            var query = _unitOfWork.bedRepository.getAllBed().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(b => (!string.IsNullOrEmpty(b.BedNumber) && b.BedNumber.ToLower().Contains(search)) ||
                                         (b.Ward != null && !string.IsNullOrEmpty(b.Ward.Name) && b.Ward.Name.ToLower().Contains(search)) ||
                                         b.BedType.ToString().ToLower().Contains(search) ||
                                         (b.RatePerDay.HasValue && b.RatePerDay.ToString().Contains(search)));
            }

            query = query.OrderBy(i => i.Id);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize).Select(b => new
            {
                id = b.Id,
                bedNumber = b.BedNumber,
                wardId = b.WardId,
                wardName = b.Ward != null ? b.Ward.Name : "",
                ratePerDay = b.RatePerDay,
                isOccupied = b.IsOccupied,
                bedType = b.BedType.ToString(),
                bedTypeId = (int)b.BedType
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

            var ward = _unitOfWork.wardRepository.GetAll();
            ViewBag.Ward = ward;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(Bed bed)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            var canAdd = await _unitOfWork.bedRepository.CanAddBedToWardAsync(bed.WardId);
            if (!canAdd)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "❌ Ward capacity reached! Cannot add more beds." });
                }
                TempData["Message"] = "❌ Target Filup";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Save");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.bedRepository.Save(bed);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Bed Successfully Added!" });
                }
                TempData["Message"] = "✅ Save Successful";
                TempData["MessageType"] = "success";
                return RedirectToAction("Save");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid bed data submitted." });
            }

            var department = _unitOfWork.departmentRepo.getAll();
            ViewBag.Department = department;

            var ward = _unitOfWork.wardRepository.GetAll();
            ViewBag.Ward = ward;

            TempData["Message"] = "❌ Invalid data submitted";
            TempData["MessageType"] = "danger";
            return View(bed);
        }

        [HttpGet]
        public IActionResult GetBedById(int id)
        {
            var bed = _unitOfWork.bedRepository.Find(id);
            if (bed == null)
            {
                return NotFound();
            }
            return Json(new
            {
                id = bed.Id,
                bedNumber = bed.BedNumber,
                wardId = bed.WardId,
                wardName = bed.Ward?.Name,
                ratePerDay = bed.RatePerDay,
                isOccupied = bed.IsOccupied,
                bedType = (int)bed.BedType,
                bedTypeName = bed.BedType.ToString()
            });
        }

        [HttpPost]
        public IActionResult Update(Bed bed)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                _unitOfWork.bedRepository.Update(bed);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Bed Successfully Updated!" });
                }
                TempData["Message"] = "✅ Successfully Updated!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid bed data submitted." });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                var isInUsed = await _unitOfWork.bedRepository.IsBedInUseAsync(id);
                if (isInUsed)
                {
                    if (isAjax)
                    {
                        return Json(new { success = false, message = "❌ Bed is currently in use by an Admission/Prescription!" });
                    }
                    TempData["Message"] = "❌ Bed Used in Admission!";
                    TempData["MessageType"] = "danger";
                    return RedirectToAction("Index");
                }

                _unitOfWork.bedRepository.Delete(id);

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

        [HttpPost]
        public IActionResult StatusUpdate(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            _unitOfWork.bedRepository.StatusUpdate(id);
            if (isAjax)
            {
                return Json(new { success = true, message = "✅ Occupancy status toggled!" });
            }
            return RedirectToAction("Index");
        }

        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var result = _unitOfWork.bedRepository.getAllBed()
                        .Where(p => p.BedNumber?.ToLower().Contains(name) == true)
                        .Select(p => new
                        {
                            Id = p.Id,
                            BedName = p.BedNumber,
                            WardName = p.Ward != null ? p.Ward.Name : "",
                            Rate = p.RatePerDay,
                            IsOccupied = p.IsOccupied,
                            Type = p.BedType.ToString()
                        })
                        .ToList();

            return Json(result);
        }
    }
}
