using HMSYSTEM.Data;
using HMSYSTEM.Models;
using System.Reflection.Metadata;

namespace HMSYSTEM.Repository
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly Db _db;

        public AppointmentRepository(Db db)
        {
            _db = db;
        }

        public List<Appointment> GetAllAppointments()
        {
           return  _db.Appointments.ToList();

        } 
    }
}
