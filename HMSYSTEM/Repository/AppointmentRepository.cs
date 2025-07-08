using HMSYSTEM.Data;
using HMSYSTEM.Models;
using System;
using System.Reflection.Metadata;
using HMSYSTEM.Enum;

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
           return  _db.Appointments.Where(p => p.Status == AppointmentStatus.Active).ToList();

        }

        public void Save(Appointment appointment)
        {
       
            _db.Appointments.Add(appointment);
            _db.SaveChanges();

        }
        public void Delete(int Id)
        {
            var appoint = _db.Appointments.Find(Id);

            appoint.Status =AppointmentStatus.Deleted;
            _db.SaveChanges();

        }
    }
}
