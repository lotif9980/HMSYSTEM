using HMSYSTEM.Enum;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAllAppointments();
        public void Save(Appointment appointment);
        public void Delete(int Id);
        List<Appointment> GetDeleteAppointments();
        List<Appointment> GetProgress();
        List<Appointment> GetComplete();
        public void UpdateStatus(int id, AppointmentStatus status);
        List<Appointment> GetSerial();

    }
}
