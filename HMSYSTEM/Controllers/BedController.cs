using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
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


        public IActionResult Index(int page=1, int pageSize=5)
        {
            var totalBeds =_unitOfWork.bedRepository.getAllBed().OrderBy(s=>s.Id);
            var totalItem=totalBeds.Count();

            var totalPage=(int)Math.Ceiling((double)totalItem/pageSize);

            var beds = totalBeds
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModels = new PaginationViewModel<Bed>
            {
                Items= beds,
                CurrentPage=page,
                TotalPages=totalPage,
                PageSize=pageSize,
                TotalItems=totalItem
                
            };
            return View(viewModels);
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

            if (ModelState.IsValid)
            {
                _unitOfWork.bedRepository.Save(bed);
                TempData["Message"] = "✅ Save Successful";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Save");
            }

            var department = _unitOfWork.departmentRepo.getAll();
            ViewBag.Department = department;

            var ward = _unitOfWork.wardRepository.GetAll();
            ViewBag.Ward = ward;

            TempData["Message"] = "❌ Invalid data submitted";
            TempData["MessageType"] = "danger";
            return View(bed);
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

        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var result = _unitOfWork.bedRepository.getAllBed()
                        .Where(p => p.BedNumber?.ToLower().Contains(name) == true)
                        .Select(p => new
                        {
                            Id=p.Id,
                            BedName=p.BedNumber,
                            WardName=p.Ward.Name,
                            Rate=p.RatePerDay,
                            IsOccupied=p.IsOccupied,
                            Type=p.BedType
                        })
                        .ToList();

            return Json(result);
        }

    }
}
