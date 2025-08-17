using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HMSYSTEM.ViewModels;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        protected readonly IUnitOfWork _unitOf;

        public DepartmentController(IUnitOfWork unitOf)
        {
            _unitOf = unitOf;
        }


        [HttpGet]
        [Authorize]
        
        public IActionResult Index(int page=1, int pageSize=5)
        {


            var totalDepartment = _unitOf.departmentRepo.getAll().OrderBy(d=>d.DepartmentId);
            var totalItem = totalDepartment.Count();
            var totalPage=(int)Math.Ceiling((decimal)totalItem / pageSize);

            var departments= totalDepartment
                            .Skip((page-1)*pageSize)
                            .Take(pageSize)
                            .ToList();

            var viewModel = new PaginationViewModel<Department>
            {
                Items = departments,
                CurrentPage=page,
                PageSize=pageSize,
                TotalItems=totalItem,
                TotalPages=totalPage,
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(Department department)
        {
            if (ModelState.IsValid) 
            { 
                _unitOf.departmentRepo.Save(department);
                TempData["Message"] = "✅ Successfully Added!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Save");
            }
            return View(department);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var data = _unitOf.departmentRepo.Edit(Id);
            return View(data);
        }

        [HttpPost]
        public IActionResult Update(Department department)
        {
           var data=  _unitOf.departmentRepo.Update(department);
            TempData["Message"] = "✅ Successfully Added!";
            TempData["MessageType"] = "primary";

            return RedirectToAction("Save",data);
        }
        public async Task<IActionResult> Delete(int Id)
        {
            var data =await _unitOf.departmentRepo.inUsedCheck(Id);
            if (data)
            {
                TempData["Message"] = "✅ Used in Department!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
            _unitOf.departmentRepo.Delete(Id);
            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";
            return RedirectToAction("Index");
        }

        public IActionResult GetNameSearch(string name)
        {
            name = name?.Trim().ToLower() ?? "";

            var data = _unitOf.departmentRepo.getAll()
                .Where(m => !string.IsNullOrEmpty(m.DepartmentName)&& m.DepartmentName.ToLower().Contains(name))
                .Select(m => new
                {
                    m.DepartmentName,
                    m.DepartmentId,
                    m.Status
                }).ToList();


            return Json(data);
        }

    }
}
