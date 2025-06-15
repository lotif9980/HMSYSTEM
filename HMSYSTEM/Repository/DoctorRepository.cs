using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class DoctorRepository : IDoctorRepository
    {
        protected readonly Db _db;
        private IWebHostEnvironment _env;
       

        public DoctorRepository(Db db,IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
            
        }


        public List<Doctor> getAll()
        {
           return _db.Doctors.Include(d=>d.Department).ToList();
        }

        public Doctor Details(int id)
        {
                return _db.Doctors
            .Include(d => d.Department)
            .Include(d => d.Designation)
            .FirstOrDefault(d => d.Id == id);

        }

        public Doctor Edit(int Id)
        {
            return _db.Doctors.FirstOrDefault(d=>d.Id==Id);
                
        }

        public List<Doctor> Save(Doctor doctor)
        {
            _db.Add(doctor);
            _db.SaveChanges();

            return _db.Doctors.ToList();
        }

        public void Update(Doctor doctor)
        {
            var existingDoctor = _db.Doctors.FirstOrDefault(d => d.Id == doctor.Id);
            if (existingDoctor != null)
            {
                _db.Entry(existingDoctor).CurrentValues.SetValues(doctor);
                _db.SaveChanges();
            }
        }


        public Doctor Find(int Id)
        {
            return _db.Doctors.Find(Id);
          
        }

        public void Delete(int id)
        {
            var doctor = _db.Doctors.Find(id);
            if (doctor != null)
            {
                // 1. Delete image file from wwwroot/uploads
                if (!string.IsNullOrEmpty(doctor.Picture))
                {
                    string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string imagePath = Path.Combine(wwwRootPath, "Doctor", doctor.Picture);

                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                // 2. Remove doctor from database
                _db.Doctors.Remove(doctor);
                _db.SaveChanges();
            }
        }
    }
}
