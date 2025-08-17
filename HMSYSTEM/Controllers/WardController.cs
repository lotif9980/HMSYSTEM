using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
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


        public IActionResult Index(int pageSize=5, int page=1)
        {
            var totalWard= _unitOfWork.wardRepository.GetAll().OrderBy(i=>i.Id);
            var totalItems=totalWard.Count();

            var totalPage=(int)Math.Ceiling((double)totalItems/pageSize);

            var wards=totalWard
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new PaginationViewModel<Ward>
            {
                Items= wards,
                CurrentPage=page,
                TotalPages=totalPage,
                PageSize=pageSize,
                TotalItems=totalItems,
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Save()
        {
            var department=_unitOfWork.departmentRepo.getAll();

            ViewBag.Department=department;
            return View();
        }

        [HttpPost]
        public IActionResult Save(Ward ward)
        {
            var department = _unitOfWork.departmentRepo.getAll();
            ViewBag.Department = department;

            if (ModelState.IsValid)
            {
                _unitOfWork.wardRepository.Save(ward);
                TempData["Message"] = "✅ Successfully Added";
                TempData["MessageType"] = "success";

                return RedirectToAction("Index");
            }
            TempData["Message"] = "❌ Invalid  data submitted.";
            TempData["MessageType"] = "danger";
            return View(ward);
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



        public IActionResult GetSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var result = _unitOfWork.wardRepository.GetAll()
                        .Where(p => p.Name?.ToLower().Contains(name) == true)
                        .Select(p => new
                        {
                            p.Name,
                            p.Id,
                            DepartmentName=p.Department.DepartmentName,
                            p.TotalBeds,
                            p.FloorNo,
                            type= p.Type.ToString(),
                        }).ToList();

            return Json(result);

        }

    }
}
