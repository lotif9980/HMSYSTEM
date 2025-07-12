using HMSYSTEM.Data;
using HMSYSTEM.Models;
using System;
using System.Reflection.Metadata;
using HMSYSTEM.Enum;
using Microsoft.EntityFrameworkCore;


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
           return  _db.Appointments.Include(d=>d.Department)
                .Include(x=>x.Patient)
                .Include(d=>d.Doctor). Where(p => p.Status == AppointmentStatus.Active).ToList();

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

        public List<Appointment> GetDeleteAppointments()
        {
            return _db.Appointments
                .Include(x=> x.Doctor)
                .Include(x=>x.Department)
                .Include(x=>x.Patient).Where(p => p.Status == AppointmentStatus.Deleted).ToList();
        }
        public List<Appointment> GetProgress()
        {
            return _db.Appointments
                .Include(x=>x.Doctor)
                .Include(x=>x.Department)
                .Include(x=>x.Patient).Where(p=>p.Status==AppointmentStatus.InProgress).ToList();
        }

        public List<Appointment> GetComplete()
        {
            return _db.Appointments
                 .Include(x => x.Doctor)
                 .Include(x => x.Department)
                 .Include(x=>x.Patient).Where(p => p.Status == AppointmentStatus.Completed).ToList();
        }


        public void UpdateStatus(int id, AppointmentStatus status)
        {
            var appointment = _db.Appointments.Find(id);
            if (appointment != null)
            {
                appointment.Status = status;
                _db.SaveChanges();
            }
        }

        public List<Appointment> GetSerial()
        {
            return _db.Appointments.Where(p=>p.Status != AppointmentStatus.Deleted).ToList();
        }
    }

}
