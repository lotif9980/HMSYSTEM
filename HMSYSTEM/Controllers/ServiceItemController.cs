using HMSYSTEM.Helpers;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {
           var data = _unitOfWork.serviceItemRepository.GetAll()
                      .OrderBy(d=>d.Id)
                      .AsQueryable();
            return View(data);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(ServiceItem serviceItem)
        {
          _unitOfWork.serviceItemRepository.Save(serviceItem);
           return RedirectToAction("Save");
        }
    }
}
