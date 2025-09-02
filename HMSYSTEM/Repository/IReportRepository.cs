using HMSYSTEM.ViewModels;

namespace HMSYSTEM.Repository
{
    public interface IReportRepository
    {
        public List<WardBedViewModel> GetWardBedStatus(int?wardId=null);

        public List<AdmissionViewModel> GetAllAdmission(DateTime fromDate, DateTime toDate);

        public List<AppointmentVM> GetAppointment(DateTime formDate, DateTime toDate);
    }
}
