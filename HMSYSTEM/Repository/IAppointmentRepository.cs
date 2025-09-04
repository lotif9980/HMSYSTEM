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
        List<Appointment> GetDeleteAppointments();
        IQueryable<Appointment> GetProgress(int? doctorId=null);
        List<Appointment> GetComplete();
        public void UpdateStatus(int id, AppointmentStatus status);
        List<Appointment> GetSerial();

        int GetAppointmentsCount();
        public Task<bool> AppointmentCheck(int PatientId);

    }
}
