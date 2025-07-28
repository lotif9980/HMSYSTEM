using HMSYSTEM.Models;
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

        [HttpGet]
        public IActionResult Save()
        {
            var department = _unitOfWork.departmentRepo.getAll();
            ViewBag.Department= department;

            var ward = _unitOfWork.wardRepository.GetAll();
            ViewBag.Ward = ward;

            return View();
        }

        [HttpPost]
        public IActionResult Save(Bed bed)
        {
            _unitOfWork.bedRepository.Save(bed);
            return RedirectToAction("Index");
        }

        public IActionResult StatusUpdate(int id)
        {
            _unitOfWork.bedRepository.StatusUpdate(id);
            return RedirectToAction("Index");
        }



    }
}
