using HMSYSTEM.Data;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        protected readonly IUnitOfWork _unitOf;
        private IWebHostEnvironment _env;

        public DoctorController(IUnitOfWork unitOf, IWebHostEnvironment env)
        {
            _unitOf = unitOf;
            _env = env;
        }


        [Authorize]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var allDoctors = _unitOf.doctorRepo.getAll(); 

            int totalCount = allDoctors.Count();

            var pagedDoctors = allDoctors
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new PaginationViewModel<Doctor>
            {
                Items = pagedDoctors,
                CurrentPage = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return View(viewModel);
        }

        public IActionResult Details(int Id)
        {
            var data = _unitOf.doctorRepo.Details(Id);
            return View(data);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var data = _unitOf.doctorRepo.Edit(Id);

            var departments = _unitOf.departmentRepo.getAll();
            var designation = _unitOf.designationRepo.getAll();

            ViewBag.Designation = designation;
            ViewBag.Department = departments;

            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(Doctor doctor)
        {
            var existingDoctor = _unitOf.doctorRepo.Find(doctor.Id);
            if (existingDoctor == null)
                return NotFound();

            string oldFileName = existingDoctor.Picture;

            // নতুন ছবি না থাকলে আগের ছবি রাখি, কিন্তু নাম পরিবর্তন হলে ফাইলও rename করব
            if (doctor.ImageFile == null)
            {
                doctor.Picture = existingDoctor.Picture;

                // FirstName বা LastName পরিবর্তন হয়েছে কি না চেক
                if (doctor.FirstName != existingDoctor.FirstName || doctor.LastName != existingDoctor.LastName)
                {
                    string firstName = doctor.FirstName.Replace(" ", "_");
                    string lastName = doctor.LastName.Replace(" ", "_");
                    string newFileName = $"{doctor.Id}_{firstName}_{lastName}{Path.GetExtension(oldFileName)}";

                    string imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Doctor");
                    string oldFilePath = Path.Combine(imageFolder, oldFileName);
                    string newFilePath = Path.Combine(imageFolder, newFileName);

                    // ফাইল rename করা
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Move(oldFilePath, newFilePath);
                    }

                    doctor.Picture = newFileName;
                }
            }

            _unitOf.doctorRepo.Update(doctor);

            TempData["Message"] = "✅ Successfully Added!";
            TempData["MessageType"] = "primary";


            _unitOf.Save();

            // যদি নতুন ছবি দেওয়া হয়
            if (doctor.ImageFile != null)
            {
                string firstName = doctor.FirstName.Replace(" ", "_");
                string lastName = doctor.LastName.Replace(" ", "_");
                string newFileName = $"{doctor.Id}_{firstName}_{lastName}{Path.GetExtension(doctor.ImageFile.FileName)}";

                // পুরনো ফাইল থাকলে ডিলিট করি
                if (!string.IsNullOrEmpty(oldFileName))
                {
                    string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Doctor", oldFileName);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }

                SavePhoto(doctor.ImageFile, newFileName);
                doctor.Picture = newFileName;

                _unitOf.doctorRepo.Update(doctor);

                TempData["Message"] = "✅ Successfully Added!";
                TempData["MessageType"] = "primary";

                _unitOf.Save();
            }

            return RedirectToAction("Index");
        }




        public IActionResult Delete(int Id)
        {
            _unitOf.doctorRepo.Delete(Id);
            TempData["Message"] = "✅ Successfully Delete!";
            TempData["MessageType"] = "danger";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Save()
        {
            var designation = _unitOf.designationRepo.getAll()
            .Where(c=>c.Status==true)
            .Select(c => new { c.DesignationId, c.DesignationName })
            .ToList();

          
            var department = _unitOf.departmentRepo.getAll()
                .Where(c=>c.Status==true)
                .Select(c => new { c.DepartmentId, c.DepartmentName })
                .ToList();

            ViewBag.Designation = designation;
            ViewBag.Department = department;
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Save(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _unitOf.doctorRepo.Save(doctor);
                await _unitOf.Save();

                if (doctor.ImageFile != null)
                {
                    string firstName = doctor.FirstName.Replace(" ", "_");
           
                    string convertName = firstName;
                    string fileName = $"{doctor.Id}_{convertName}{Path.GetExtension(doctor.ImageFile.FileName)}";

                    SavePhoto(doctor.ImageFile, fileName);

                    doctor.Picture = fileName;

                    _unitOf.doctorRepo.Update(doctor);
                    await _unitOf.Save();
                }

                var User = new User
                {
                    Name=doctor.FirstName +" "+doctor.LastName,
                    MobileNo=doctor.PhoneNo,
                    UserName = doctor.EmailAddress,
                    Password = doctor.Password,
                    RoleId=_unitOf.RoleRepository.GetRoles().FirstOrDefault(r=>r.Name== "Doctor")?.Id,
                    DoctorId=doctor.Id
                };

                _unitOf.UserRepository.Save(User);

                TempData["Message"] = "✅ Successfully Added Doctor and User!";
                TempData["MessageType"] = "primary";

                return RedirectToAction("Save");
            }

            var designation = _unitOf.designationRepo.getAll()
            .Where(c => c.Status == true)
            .Select(c => new { c.DesignationId, c.DesignationName })
            .ToList();


            var department = _unitOf.departmentRepo.getAll()
                .Where(c => c.Status == true)
                .Select(c => new { c.DepartmentId, c.DepartmentName })
                .ToList();

            ViewBag.Designation = designation;
            ViewBag.Department = department;

            return View(doctor);
        }


        private void SavePhoto(IFormFile file, string name)
        {
            string path = Path.Combine(_env.ContentRootPath, "wwwroot", "Doctor");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fullPath = Path.Combine(path, name);

            using (FileStream fs = System.IO.File.Create(fullPath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }
        }


        public IActionResult GetDoctorPrintPartial(int id)
        {
            var doctor = _unitOf.doctorRepo.Details(id);

            if (doctor == null) return NotFound();

            return PartialView("_DoctorPrintPartial", doctor);
        }


        public IActionResult GetName(string name)
        {
            name=(name ?? "").Trim().ToLower();

            var result=_unitOf.doctorRepo.getAll()
               .Where(d =>
                    ((d.FirstName ?? "").Trim().ToLower().Contains(name)) ||
                    ((d.LastName ?? "").Trim().ToLower().Contains(name)) ||
                    (((d.FirstName ?? "") + " " + (d.LastName ?? "")).Trim().ToLower().Contains(name))
               ).Select(p => new
                {
                    p.Id,
                    Name= p.FirstName +" "+ p.LastName,
                    Department = p.Department.DepartmentName,
                    p.Picture,
                    p.EmailAddress,
                    p.Status,
                }).ToList();

            return Json(result);
        }

    }
}
