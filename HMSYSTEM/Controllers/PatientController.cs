using Microsoft.AspNetCore.Mvc;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using HMSYSTEM.ViewModels;
using HMSYSTEM.Helpers;
using Microsoft.EntityFrameworkCore;


namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {

        protected readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public PatientController(IUnitOfWork unit, IWebHostEnvironment webHostEnvironment)
        {
            _unit = unit;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPatients(string search = "", int page = 1, int pageSize = 10)
        {
            var query = _unit.PatienRepo.getAll().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(p => 
                    (!string.IsNullOrEmpty(p.FirstName) && p.FirstName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(p.LastName) && p.LastName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(p.Phone) && p.Phone.Contains(search)) ||
                    (!string.IsNullOrEmpty(p.Email) && p.Email.ToLower().Contains(search))
                );
            }

            query = query.OrderBy(p => p.PatientID);

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(p => new
                {
                    patientID = p.PatientID,
                    firstName = p.FirstName,
                    lastName = p.LastName,
                    email = p.Email,
                    phone = p.Phone,
                    address = p.Address,
                    bloodGroup = p.BloodGroup,
                    dateOfBirth = p.DateOfBirth.HasValue ? p.DateOfBirth.Value.ToString("d-MMM-yyyy") : "N/A",
                    status = p.Status,
                    picture=p.Picture
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
        public IActionResult GetPatientById(int id)
        {
            var p = _unit.PatienRepo.Edit(id);
            if (p == null)
            {
                return NotFound();
            }
            return Json(new
            {
                patientID = p.PatientID,
                firstName = p.FirstName,
                lastName = p.LastName,
                email = p.Email,
                phone = p.Phone,
                bloodGroup = p.BloodGroup,
                sex = p.Sex != null ? p.Sex.Trim() : "",
                fatherName = p.FatherName,
                dateOfBirth = p.DateOfBirth.HasValue ? p.DateOfBirth.Value.ToString("yyyy-MM-dd") : "",
                address = p.Address,
                emergencyContact = p.EmergencyContact,
                status = p.Status,
                picture = p.Picture
            });
        }

        [HttpPost]
        public IActionResult Save(Patient patient)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (ModelState.IsValid)
            {
                if (patient.ImageFile != null && patient.ImageFile.Length > 0)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(patient.ImageFile.FileName);
                    string path = Path.Combine(wwwRootPath, "Patients", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        patient.ImageFile.CopyTo(stream);
                    }
                    patient.Picture = fileName;
                }

                try
                {
                    _unit.PatienRepo.Save(patient);
                    if (isAjax)
                    {
                        return Json(new { success = true, message = "✅ Successfully Added!" });
                    }
                    TempData["Message"] = "✅ Successfully Added!";
                    TempData["MessageType"] = "primary";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException ex)
                {
                    string errorMsg = "❌ Unexpected error occurred!";
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("UQ_Patient_Mobile"))
                    {
                        errorMsg = "❌ This mobile number already exists!";
                    }

                    if (isAjax)
                    {
                        return Json(new { success = false, message = errorMsg });
                    }
                    TempData["Message"] = errorMsg;
                    TempData["MessageType"] = "danger";
                    return View(patient);
                }
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid Patient data submitted." });
            }
            return View(patient);
        }

        [HttpPost]
        public IActionResult Edit(Patient model)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            
                var existing = _unit.PatienRepo.Edit(model.PatientID);
                if (existing == null)
                {
                    if (isAjax) return Json(new { success = false, message = "❌ Patient not found." });
                    return NotFound();
                }

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(existing.Picture) && existing.Picture != "default.png")
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Patients", existing.Picture);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Patients", fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.ImageFile.CopyTo(stream);
                    }
                    model.Picture = fileName;
                }
                else
                {
                    model.Picture = existing.Picture;
                }

                _unit.PatienRepo.Update(model);

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully Updated!" });
                }
                TempData["Message"] = "✅ Successfully Updated!";
                TempData["MessageType"] = "primary";
                return RedirectToAction("Index");
            

            if (isAjax)
            {
                return Json(new { success = false, message = "❌ Invalid Patient data submitted." });
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Delete(int Id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                var existing = _unit.PatienRepo.Edit(Id);
                if (existing != null && !string.IsNullOrEmpty(existing.Picture) && existing.Picture != "default.png")
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Patients", existing.Picture);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _unit.PatienRepo.Delete(Id);

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Successfully Deleted!" });
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


        public IActionResult PrintGetPatient(int id)
        {

            var patient=_unit.PatienRepo.Details(id);

            return PartialView("_PatientPrintPartial", patient);
        }

        public IActionResult GetName(string name)
        {
            name = (name ?? "").Trim().ToLower();

            var result = _unit.PatienRepo.getAll()
                  .Where(p =>
                    ((p.FirstName ?? "").Trim().ToLower().Contains(name)) ||
                    ((p.LastName ?? "").Trim().ToLower().Contains(name)) ||
                    (((p.FirstName ?? "") + " " + (p.LastName ?? "")).Trim().ToLower().Contains(name))
                  )
                .Select(p => new
                {
                    p.PatientID,
                    name=  p.FirstName +" "+ p.LastName,
                    p.DateOfBirth,
                    p.Email,
                    p.Phone,
                    p.Address,
                    p.BloodGroup,
                    p.Status
                }).ToList();

            return Json(result);
        }
    }
}
