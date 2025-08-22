using HMSYSTEM.Helpers;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class ServiceItemController : Controller
    {
        protected readonly IUnitOfWork _unitOfWork;
        public ServiceItemController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
           var data = _unitOfWork.serviceItemRepository.GetAll()
                      .OrderBy(d=>d.Id)
                      .AsQueryable()
                      .ToPagedList(page,pageSize);
            return View(data);

        }
    }
}
