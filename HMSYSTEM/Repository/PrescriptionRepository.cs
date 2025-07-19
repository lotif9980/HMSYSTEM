using HMSYSTEM.Data;
using HMSYSTEM.Models;
using HMSYSTEM.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class PrescriptionRepository:IPrescriptionRepository
    {
        private readonly Db _db;


        public PrescriptionRepository(Db db)
        {
            _db = db;
        }

      

        public List<Prescription> GetAll()
        {
            return _db.Prescriptions
                .Include(d=>d.Doctor)
                .Include(d=>d.Department)
                .Include(p=>p.Patient).ToList();
        }

        public void Save(Prescription prescription)
        {
            _db.Prescriptions.Add(prescription);
            _db.SaveChanges();
        }

        public void  Delete(int id)
        {
            //var data =_db.Prescriptions.Find(id);
            var data=_db.Prescriptions.Include(p=>p.PrescriptionDetails).FirstOrDefault(p=>p.Id == id);

            _db.PrescriptionDetails.RemoveRange(data.PrescriptionDetails);
            _db.Prescriptions.Remove(data);
            _db.SaveChanges();
        }

        public PrescriptionViewModel? GetPrescriptionViewModel(int id)
        {
            var data = _db.Prescriptions
    .Include(p => p.Patient)
    .Include(p => p.Doctor)
    .Include(p => p.Department)
    .Include(p => p.PrescriptionDetails)
        .FirstOrDefault(p => p.Id == id);

            if (data == null) return null;

            var viewModel = new PrescriptionViewModel
            {
                Id = data.Id,
                Date = data.Date,
                PatientId = data.PatientId,
                PatientName = data.Patient?.FirstName + " " + data.Patient?.LastName,
                DoctorId = data.DoctorId,
                DoctorName = data.Doctor?.FirstName + " " + data.Doctor?.LastName,
                DepartmentId = data.DepartmentId,
                DepartmentName = data.Department?.DepartmentName,
                Status = data.Status,
                Note = data.Note,
                NextFlowUp = data.NextFlowUp,
                PrescriptionDetails = data.PrescriptionDetails.Select(d => new PrescriptionDetailViewModel
                {
                    Id = d.Id,
                    PrescriptionId = d.PrescriptionId,
                    MedicineId = d.MedicineId,
                    MedicineName = d.Medicine?.Name, // Medicine.Include না করলে এখানে error আসবে
                    Dose = d.Dose,
                    Duration = d.Duration,
                    Instructions = d.Instructions
                }).ToList()
            };

            return viewModel;
        }



    }
}
