using AspNetCoreGeneratedDocument;
using HMSYSTEM.Data;
using HMSYSTEM.Enum;
using HMSYSTEM.Models;
using HMSYSTEM.ViewModels;

namespace HMSYSTEM.Repository
{
    public class ReportRepository : IReportRepository
    {
        private protected readonly Db _db;

        public ReportRepository(Db db)
        {
            _db = db;
        }

       

        public List<WardBedViewModel> GetWardBedStatus(int? wardId = null)
        {
            var data = from w in _db.Wards
                       where !wardId.HasValue || w.Id == wardId.Value
                       from b in _db.Beds.Where(b => b.WardId == w.Id).DefaultIfEmpty()
                       select new WardBedViewModel
                       {
                           WardName = w.Name,
                           BedNumber = b != null ? b.BedNumber : "-",
                           IsOccupied=b.IsOccupied
                       };

            return data.ToList();
        }

        public List<AdmissionViewModel> GetAllAdmission(DateTime fromDate, DateTime toDate)
        {
            var viewModel = (from a in _db.Admissions
                             join p in _db.Patients on a.PatientId equals p.PatientID
                             join d in _db.Doctors on a.DoctorId equals d.Id into dgroup
                             from d in dgroup.DefaultIfEmpty()
                             join b in _db.Beds on a.BedId equals b.Id into bgroup
                             from b in bgroup.DefaultIfEmpty()
                             where a.Status== 1 && a.AdmitDate >= fromDate
                           && a.AdmitDate <= toDate
                             select new AdmissionViewModel
                             {
                                 PatientName = p.FirstName + " " + p.LastName,
                                 PatientPhoneNumber=p.Phone,
                                 DoctorName = d.FirstName + " " + d.LastName,
                                 BedName = b.BedNumber,
                                 AdmitDate = a.AdmitDate,
                                 InvoiceNo = a.InvoiceNo,
                                 ForReason = a.ForReason
                             }).ToList();
            return viewModel;
        }

        public List<AppointmentVM> GetAppointment(DateTime formDate, DateTime toDate)
        {
           var viewModel=(from a in _db.Appointments
                         join p in _db.Patients on a.PatientID equals p.PatientID into gPatientgroup
                         from p in gPatientgroup.DefaultIfEmpty()
                         join d in _db.Doctors on a.DoctorId equals d.Id
                         where  a.AppoinmentDate >=formDate && a.AppoinmentDate <= toDate
                         select new AppointmentVM 
                         { 
                             PatientName=p.FirstName + " " + p.LastName,
                             PatientPhoneNumber=p.Phone,
                             Problem=a.Problem,
                             SerialNumber=a.SerialNumber,
                             AppoinmentDate=a.AppoinmentDate
                         }).ToList();
            return viewModel;
        }

        public List<PrescriptionViewModel> GetPrescriptions(DateTime formDate, DateTime toDate)
        {
            var viewModel = (from p in _db.Prescriptions
                             join pat in _db.Patients on p.PatientId equals pat.PatientID
                             join doc in _db.Doctors on p.DoctorId equals doc.Id
                             where p.Date>=formDate && p.Date<=toDate
                             select new PrescriptionViewModel
                             {
                                 PatientName=pat.FirstName + " " + pat.LastName,
                                 PatientMobileNo=pat.Phone,
                                 DoctorName=doc.FirstName +""+ doc.LastName,
                                 Date=p.Date,
                                 NextFlowUp=p.NextFlowUp
                             }).ToList();

            return viewModel;
        }

        public List<BillViewModel> GetBill(DateTime? fromDate, DateTime? toDate)
        {
           var viewModel=(from b in _db.Bills
                          join p in _db.Patients on b.PatientId equals  p.PatientID
                          where b.BillDate>=fromDate && b.BillDate<=toDate
                          select new BillViewModel
                          {
                              BillNo=b.BillNo,
                              BillDate=b.BillDate,
                              PatientName=p.FirstName+""+p.LastName,
                              TotalAmount=b.TotalAmount,
                              Discount=b.Discount,
                              NetAmount=b.NetAmount,
                              DueAmount=b.DueAmount,
                              PaymentAmt=b.PaymentAmt,
                              PatientPhoneNumber=p.Phone
                          }).ToList();

            return viewModel;
        }
    }
}
