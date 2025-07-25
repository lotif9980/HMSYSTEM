using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class BedController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BedController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            var data =_unitOfWork.bedRepository.getAllBed();
            return View(data);
        }
    }
}
