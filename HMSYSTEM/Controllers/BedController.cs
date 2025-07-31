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
        public async Task<IActionResult> Save(Bed bed)
        {

            var canAdd = await _unitOfWork.bedRepository.CanAddBedToWardAsync(bed.WardId);
            if (!canAdd)
            {
                TempData["Message"] = "✅ Target Filup";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Save");
            }

             _unitOfWork.bedRepository.Save(bed);
            TempData["Message"] = "✅ Save Successful";
            TempData["MessageType"] = "danger";
            return RedirectToAction("Save");
        }

        public IActionResult StatusUpdate(int id)
        {
            _unitOfWork.bedRepository.StatusUpdate(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var isInUsed = await _unitOfWork.bedRepository.IsBedInUseAsync(id);
            if (isInUsed)
            {
                TempData["Message"] = "✅ Bed Used in Prescription!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");

            }
            _unitOfWork.bedRepository.Delete(id);
            
            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";

            return RedirectToAction("Index");
        }

    }
}
