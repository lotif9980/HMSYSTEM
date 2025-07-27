using HMSYSTEM.Data;
using HMSYSTEM.Models;


namespace HMSYSTEM.Repository
{
    public class PatientRepository : IPatientRepository
    {
        protected readonly Db _db;
        private readonly IWebHostEnvironment _env;

        public PatientRepository(Db db , IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        

        public List<Patient> getAll()
        {
           return _db.Patients.ToList();
        }


        public Patient Details(int Id)
        {
            return _db.Patients.Find(Id);
        }

        public List<Patient> Delete(int Id)
        {
            var data = _db.Patients.Find(Id);
            _db.Remove(data);
            _db.SaveChanges();

            return _db.Patients.ToList();
        }

        public Patient Edit(int Id)
        {
            return _db.Patients.Find(Id);
        }

        public List<Patient> Save(Patient patient)
        {
            _db.Add(patient);
            _db.SaveChanges();

            return _db.Patients.ToList();
        }

        public Patient Update(Patient patient)
        {
            var existing = _db.Patients.Find(patient.PatientID);
            if (existing == null) return null;

            existing.FirstName = patient.FirstName;
            existing.LastName = patient.LastName;
            existing.Email = patient.Email;
            existing.Password = patient.Password;
            existing.Phone = patient.Phone;
            existing.BloodGroup = patient.BloodGroup;
            existing.Sex = patient.Sex;
            existing.DateOfBirth = patient.DateOfBirth;
            existing.Address = patient.Address;
            existing.EmergencyContact = patient.EmergencyContact;
            existing.FatherName = patient.FatherName;

            
            if (!string.IsNullOrEmpty(patient.Picture))
            {
                existing.Picture = patient.Picture;
            }

            existing.Status = patient.Status;

            _db.SaveChanges();
            return existing;
        }

    }
}
