using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAllAppointments();

    }
}
