using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public interface IAppointmentRepository
    {
        List<Appointment> GetAllAppointments();
        public void Save(Appointment appointment);
        public void Delete(int Id);
        
    }
}
