using HMSYSTEM.Enum;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public interface IAppointmentRepository
    {
        //IQueryable<Appointment> GetAllAppointments(DateTime? fromDate = null, DateTime? toDate = null);
        IQueryable<Appointment> GetAppointmentsByDoctorId(int? doctorId =null,DateTime? fromDate = null, DateTime? toDate = null);
        public void Save(Appointment appointment);
        public void Delete(int Id);
        IQueryable<Appointment> GetDeleteAppointments(int?doctorId=null);
        IQueryable<Appointment> GetProgress(int? doctorId=null);
        IQueryable<Appointment> GetComplete(int? doctorId=null);
        public void UpdateStatus(int id, AppointmentStatus status);
        List<Appointment> GetSerial();

        int GetAppointmentsCount();
        public Task<bool> AppointmentCheck(int PatientId);

    }
}
