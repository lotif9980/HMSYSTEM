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

        //public PrescriptionViewModel? GetPrescriptionViewModel(int id)
        //{
        //    var result = (from ps in _db.Prescriptions
        //                  join pd in _db.PrescriptionDetails on ps.Id equals pd.PrescriptionId
        //                  join pt in _db.Patients on ps.PatientId equals pt.PatientID
        //                  join ds in _db.Departments on ps.DepartmentId equals ds.DepartmentId
        //                  join ms in _db.Medicines on pd.MedicineId equals ms.Id into mGroup
        //                  from ms in mGroup.DefaultIfEmpty() // optional if MedicineId is nullable
        //                  where ps.Id == id
        //                  select new
        //                  {
        //                      Prescription = ps,
        //                      Patient = pt,
        //                      Department = ds,
        //                      Detail = pd,
        //                      Medicine = ms
        //                  }).ToList();

        //    if (!result.Any()) return null;

        //    var first = result.First();

        //    var viewModel = new PrescriptionViewModel
        //    {
        //        Id = first.Prescription.Id,
        //        Date = first.Prescription.Date,
        //        PatientId = first.Patient.PatientID,
        //        PatientName = first.Patient.FirstName + " " + first.Patient.LastName,
        //        DoctorId = first.Prescription.DoctorId,
        //        DoctorName = _db.Doctors
        //                      .Where(d => d.Id == first.Prescription.DoctorId)
        //                      .Select(d => d.FirstName + " " + d.LastName)
        //                      .FirstOrDefault(),
        //        DepartmentId = first.Department.DepartmentId,
        //        DepartmentName = first.Department.DepartmentName,
        //        Status = first.Prescription.Status,
        //        Note = first.Prescription.Note,
        //        NextFlowUp = first.Prescription.NextFlowUp,
        //        PrescriptionDetails = result.Select(r => new PrescriptionDetailViewModel
        //        {
        //            Id = r.Detail.Id,
        //            PrescriptionId = r.Detail.PrescriptionId,
        //            MedicineId = r.Detail.MedicineId,
        //            MedicineName = r.Medicine?.Name, // Null check in case of left join
        //            Dose = r.Detail.Dose,
        //            Duration = r.Detail.Duration,
        //            Instructions = r.Detail.Instructions
        //        }).ToList()
        //    };

        //    return viewModel;
        //}

        //public PrescriptionViewModel? GetPrescriptionViewModel(int id)
        //{
        //    var prescription = _db.Prescriptions
        //        .Include(p => p.Patient)
        //        .Include(p => p.Doctor)
        //        .Include(p => p.Department)
        //        .Include(p => p.PrescriptionDetails)
        //            .ThenInclude(pd => pd.Medicine)
        //        .FirstOrDefault(p => p.Id == id);

        //    if (prescription == null) return null;

        //    var viewModel = new PrescriptionViewModel
        //    {
        //        Id = prescription.Id,
        //        Date = prescription.Date,
        //        PatientId = prescription.PatientId,
        //        PatientName = prescription.Patient.FirstName + " " + prescription.Patient.LastName,
        //        DoctorId = prescription.DoctorId,
        //        DoctorName = prescription.Doctor != null ? prescription.Doctor.FirstName + " " + prescription.Doctor.LastName : null,
        //        DepartmentId = prescription.DepartmentId ,
        //        DepartmentName = prescription.Department?.DepartmentName,
        //        Status = prescription.Status,
        //        Note = prescription.Note,
        //        NextFlowUp = prescription.NextFlowUp,
        //        PrescriptionDetails = prescription.PrescriptionDetails.Select(pd => new PrescriptionDetailViewModel
        //        {
        //            Id = pd.Id,
        //            PrescriptionId = pd.PrescriptionId,
        //            MedicineId = pd.MedicineId,
        //            MedicineName = pd.Medicine?.Name,
        //            Dose = pd.Dose,
        //            Duration = pd.Duration,
        //            Instructions = pd.Instructions
        //        }).ToList()
        //    };

        //    return viewModel;
        //}


        public PrescriptionViewModel? GetPrescriptionViewModel(int id)
        {
            var prescriptionQuery =
                (from p in _db.Prescriptions
                 join pt in _db.Patients on p.PatientId equals pt.PatientID
                 join d in _db.Doctors on p.DoctorId equals d.Id into dgroup
                 from d in dgroup.DefaultIfEmpty()
                 join dep in _db.Departments on p.DepartmentId equals dep.DepartmentId into depgroup
                 from dep in depgroup.DefaultIfEmpty()
                 where p.Id == id
                 select new
                 {
                     Prescription = p,
                     Patient = pt,
                     Doctor = d,
                     Department = dep
                 }).FirstOrDefault();

            if (prescriptionQuery == null) return null;

            // এখন PrescriptionDetails ও Medicine গুলো fetch করতে হবে
            var detailsQuery =
                (from pd in _db.PrescriptionDetails
                 join m in _db.Medicines on pd.MedicineId equals m.Id into mgroup
                 from m in mgroup.DefaultIfEmpty()
                 where pd.PrescriptionId == id
                 select new PrescriptionDetailViewModel
                 {
                     Id = pd.Id,
                     PrescriptionId = pd.PrescriptionId,
                     MedicineId = pd.MedicineId,
                     MedicineName = m != null ? m.Name : null,
                     Dose = pd.Dose,
                     Duration = pd.Duration,
                     Instructions = pd.Instructions
                 }).ToList();

            var viewModel = new PrescriptionViewModel
            {
                Id = prescriptionQuery.Prescription.Id,
                Date = prescriptionQuery.Prescription.Date,
                PatientId = prescriptionQuery.Patient.PatientID,
                PatientName = prescriptionQuery.Patient.FirstName + " " + prescriptionQuery.Patient.LastName,
                DoctorId = prescriptionQuery.Prescription.DoctorId,
                DoctorName = prescriptionQuery.Doctor != null ? prescriptionQuery.Doctor.FirstName + " " + prescriptionQuery.Doctor.LastName : null,
                DepartmentId = prescriptionQuery.Department != null ? prescriptionQuery.Department.DepartmentId : 0,
                DepartmentName = prescriptionQuery.Department?.DepartmentName,
                Status = prescriptionQuery.Prescription.Status,
                Note = prescriptionQuery.Prescription.Note,
                NextFlowUp = prescriptionQuery.Prescription.NextFlowUp,
                PrescriptionDetails = detailsQuery
            };

            return viewModel;
        }

    }
}
