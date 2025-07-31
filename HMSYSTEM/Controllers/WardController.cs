using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HMSYSTEM.Controllers
{
    public class WardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public WardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            var data= _unitOfWork.wardRepository.GetAll();
            return View(data);
        }

        public IActionResult Save()
        {
            var department=_unitOfWork.departmentRepo.getAll();

            ViewBag.Department=department;
            return View();
        }

        [HttpPost]
        public IActionResult Save(Ward ward)
        {
            _unitOfWork.wardRepository.Save(ward);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var data =await _unitOfWork.wardRepository.IsBedinUsed(id);

            if (data)
            {
                TempData["Message"] = "✅ Ward Used in Bed!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
            
            _unitOfWork.wardRepository.Delete(id);

            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var data=  _unitOfWork.wardRepository.Find(id);
            
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
            var occupiedBeds = beds.Count(b => b.isOccupied==false);
            var emptyBeds = beds.Count(b => b.isOccupied == true);

            return Json(new
            {
                totalBeds,
                emptyBeds,
                occupiedBeds,
                beds
            });
        }

    }
}
