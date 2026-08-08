using HMSYSTEM.Helpers;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class ServiceItemController : Controller
    {
        protected readonly IUnitOfWork _unitOfWork;

        public ServiceItemController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetServiceItem(string search = "", int page = 1, int pageSize = 10)
        {
            var query = _unitOfWork.serviceItemRepository.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(s => (!string.IsNullOrEmpty(s.ItemName) && s.ItemName.ToLower().Contains(search)) ||
                                         (s.Amount.HasValue && s.Amount.ToString().Contains(search)));
            }

            query = query.OrderBy(i => i.Id);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize).Select(s => new
            {
                id = s.Id,
                itemName = s.ItemName,
                amount = s.Amount,
                isActive = s.IsActive
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
            return View();
        }

        [HttpPost]
        public IActionResult Save(ServiceItem serviceItem)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                serviceItem.IsActive = true;
                _unitOfWork.serviceItemRepository.Save(serviceItem);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Service Item Successfully Added!" });
                }
                TempData["Message"] = "✅ Successfully Added!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Save Failed. Please check inputs." });
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetServiceItemById(int id)
        {
            var item = _unitOfWork.serviceItemRepository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return Json(new
            {
                id = item.Id,
                itemName = item.ItemName,
                amount = item.Amount,
                isActive = item.IsActive
            });
        }

        [HttpPost]
        public IActionResult Update(ServiceItem serviceItem)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                _unitOfWork.serviceItemRepository.Update(serviceItem);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Service Item Successfully Updated!" });
                }
                TempData["Message"] = "✅ Successfully Updated!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid data submitted." });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                _unitOfWork.serviceItemRepository.Delete(id);
                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Service Item Successfully Deleted!" });
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
    }
}
