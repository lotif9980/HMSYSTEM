using Microsoft.AspNetCore.Mvc;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Hosting;


namespace HMSYSTEM.Controllers
{
    public class PatientController : Controller
    {

        protected readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public PatientController(IUnitOfWork unit, IWebHostEnvironment webHostEnvironment)
        {
            _unit = unit;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var data=  _unit.PatienRepo.getAll();
            return View(data);
        }

        [HttpGet]
        public IActionResult Details(int Id)
        {
            var data = _unit.PatienRepo.Details(Id);
            return View(data);
        }

        public IActionResult Delete(int Id)
        {
            _unit.PatienRepo.Delete(Id);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Save()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(Patient patient)
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

            _unit.PatienRepo.Save(patient);
            return RedirectToAction("Save");
        }



        public IActionResult Edit(int Id)
        {
            var data = _unit.PatienRepo.Edit(Id);

            if (!string.IsNullOrWhiteSpace(data.Sex))
            {
                data.Sex = data.Sex.Trim();
            }

            return View(data);
        }

        [HttpPost]
        public IActionResult Edit(Patient model)
        {
            var existing = _unit.PatienRepo.Edit(model.PatientID); 

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                
                if (!string.IsNullOrEmpty(existing.Picture))
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

            return RedirectToAction("Index");
        }




    }
}
