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
        [HttpGet]
        public IActionResult Index()
        {
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

            return View();
        }

        [HttpGet]
        public IActionResult GetDoctors(string search = "", int page = 1, int pageSize = 10)
        {
            var query = _unitOf.doctorRepo.getAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(d =>
                    (!string.IsNullOrEmpty(d.FirstName) && d.FirstName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(d.LastName) && d.LastName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(d.EmailAddress) && d.EmailAddress.ToLower().Contains(search)) ||
                    (d.Department != null && !string.IsNullOrEmpty(d.Department.DepartmentName) && d.Department.DepartmentName.ToLower().Contains(search))
                );
            }

            query = query.OrderBy(d => d.Id);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(d => new
                {
                    id = d.Id,
                    picture = d.Picture,
                    firstName = d.FirstName,
                    lastName = d.LastName,
                    department = d.Department != null ? d.Department.DepartmentName : "N/A",
                    designation = d.Designation != null ? d.Designation.DesignationName : "N/A",
                    emailAddress = d.EmailAddress,
                    status = d.Status
                })
                .ToList();

            return Json(new
            {
                issuccess = true,
                totalPages = totalPages,
                totalItem = totalItem,
                currentPage = page,
                data = data
            });
        }

        [HttpGet]
        public IActionResult GetDoctorById(int id)
        {
            var d = _unitOf.doctorRepo.Find(id);
            if (d == null)
            {
                return NotFound();
            }
            return Json(new
            {
                id = d.Id,
                firstName = d.FirstName,
                lastName = d.LastName,
                emailAddress = d.EmailAddress,
                password = d.Password,
                designationId = d.DesignationId,
                departmentId = d.DepartmentId,
                address = d.Address,
                phoneNo = d.PhoneNo,
                shortBiography = d.ShortBiography,
                specialist = d.Specialist,
                dateofBirth = d.DateofBirth.HasValue ? d.DateofBirth.Value.ToString("yyyy-MM-dd") : "",
                sex = d.Sex != null ? d.Sex.Trim() : "",
                picture = d.Picture,
                status = d.Status
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save(Doctor doctor)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                _unitOf.doctorRepo.Save(doctor);
                await _unitOf.Save();

                if (doctor.ImageFile != null)
                {
                    string firstName = doctor.FirstName.Replace(" ", "_");
                    string fileName = $"{doctor.Id}_{firstName}{Path.GetExtension(doctor.ImageFile.FileName)}";

                    SavePhoto(doctor.ImageFile, fileName);
                    doctor.Picture = fileName;

                    _unitOf.doctorRepo.Update(doctor);
                    await _unitOf.Save();
                }

                var User = new User
                {
                    Name = doctor.FirstName + " " + doctor.LastName,
                    MobileNo = doctor.PhoneNo,
                    UserName = doctor.EmailAddress,
                    Password = doctor.Password,
                    RoleId = _unitOf.RoleRepository.GetRoles().FirstOrDefault(r => r.Name == "Doctor")?.Id,
                    DoctorId = doctor.Id
                };

                _unitOf.UserRepository.Save(User);
                await _unitOf.Save();

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Doctor and User Profile Successfully Created!" });
                }

                TempData["Message"] = "✅ Successfully Added Doctor and User!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid Doctor data submitted." });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Doctor doctor)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                var existingDoctor = _unitOf.doctorRepo.Find(doctor.Id);
                if (existingDoctor == null)
                {
                    if (isAjax) return Json(new { success = false, message = "❌ Doctor not found." });
                    return NotFound();
                }

                string oldFileName = existingDoctor.Picture;

                if (doctor.ImageFile == null)
                {
                    doctor.Picture = existingDoctor.Picture;

                    if (doctor.FirstName != existingDoctor.FirstName || doctor.LastName != existingDoctor.LastName)
                    {
                        if (!string.IsNullOrEmpty(oldFileName))
                        {
                            string firstName = doctor.FirstName.Replace(" ", "_");
                            string lastName = doctor.LastName.Replace(" ", "_");
                            string newFileName = $"{doctor.Id}_{firstName}_{lastName}{Path.GetExtension(oldFileName)}";

                            string imageFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Doctor");
                            string oldFilePath = Path.Combine(imageFolder, oldFileName);
                            string newFilePath = Path.Combine(imageFolder, newFileName);

                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Move(oldFilePath, newFilePath);
                            }

                            doctor.Picture = newFileName;
                        }
                    }
                }

                _unitOf.doctorRepo.Update(doctor);
                await _unitOf.Save();

                if (doctor.ImageFile != null)
                {
                    string firstName = doctor.FirstName.Replace(" ", "_");
                    string lastName = doctor.LastName.Replace(" ", "_");
                    string newFileName = $"{doctor.Id}_{firstName}_{lastName}{Path.GetExtension(doctor.ImageFile.FileName)}";

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
                    await _unitOf.Save();
                }

                // Sync associated User profile
                var existingUser = _unitOf.UserRepository.GetAll().FirstOrDefault(u => u.DoctorId == doctor.Id);
                if (existingUser != null)
                {
                    existingUser.Name = doctor.FirstName + " " + doctor.LastName;
                    existingUser.MobileNo = doctor.PhoneNo;
                    existingUser.UserName = doctor.EmailAddress;
                    existingUser.Password = doctor.Password;
                    _unitOf.UserRepository.Update(existingUser);
                    await _unitOf.Save();
                }

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Doctor and User Profile Successfully Updated!" });
                }

                TempData["Message"] = "✅ Successfully Updated Doctor and User!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Index");
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid Doctor data submitted." });
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int Id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                var d = _unitOf.doctorRepo.Find(Id);
                if (d != null && !string.IsNullOrEmpty(d.Picture))
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Doctor", d.Picture);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                _unitOf.doctorRepo.Delete(Id);
                await _unitOf.Save();

                // Delete associated user
                var assocUser = _unitOf.UserRepository.GetAll().FirstOrDefault(u => u.DoctorId == Id);
                if (assocUser != null)
                {
                    _unitOf.UserRepository.Delete(assocUser.Id);
                    await _unitOf.Save();
                }

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Doctor and Associated User Profile Successfully Deleted!" });
                }
                TempData["Message"] = "✅ Successfully Deleted!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "❌ " + (ex?.Message ?? "An error occurred.") });
                }
                TempData["Message"] = "❌ " + (ex?.Message ?? "An error occurred.");
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Details(int Id)
        {
            var data = _unitOf.doctorRepo.Details(Id);
            return View(data);
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
            name = (name ?? "").Trim().ToLower();

            var result = _unitOf.doctorRepo.getAll()
               .Where(d =>
                    ((d.FirstName ?? "").Trim().ToLower().Contains(name)) ||
                    ((d.LastName ?? "").Trim().ToLower().Contains(name)) ||
                    (((d.FirstName ?? "") + " " + (d.LastName ?? "")).Trim().ToLower().Contains(name))
               ).Select(p => new
               {
                   p.Id,
                   Name = p.FirstName + " " + p.LastName,
                   Department = p.Department != null ? p.Department.DepartmentName : "N/A",
                   p.Picture,
                   p.EmailAddress,
                   p.Status,
               }).ToList();

            return Json(result);
        }
    }
}
