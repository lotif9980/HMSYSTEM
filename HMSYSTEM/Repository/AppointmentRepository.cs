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

        //public IQueryable<Appointment> GetAllAppointments(DateTime? fromDate = null, DateTime? toDate = null)
        //{
        //    var query = _db.Appointments
        //        .Include(a => a.Department)
        //        .Include(a => a.Patient)
        //        .Include(a => a.Doctor)
        //        .Where(a => a.Status == AppointmentStatus.Active)
        //        .AsQueryable();

        //    // Default current month
        //    fromDate ??= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        //    toDate ??= fromDate.Value.AddMonths(1).AddDays(-1);

        //    // Filter by only date part
        //    query = query.Where(a =>
        //        a.AppoinmentDate.Date >= fromDate.Value.Date &&
        //        a.AppoinmentDate.Date <= toDate.Value.Date);

        //    return query;
        //}

        public IQueryable<Appointment> GetAppointmentsByDoctorId(int? doctorId=null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _db.Appointments
                .Include(a => a.Department)
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Where(a => a.Status == AppointmentStatus.Active)
                .AsQueryable();

            if(doctorId.HasValue && doctorId.Value > 0)
            {
                query = query.Where(a => a.DoctorId == doctorId.Value);
            }
            // Default current month
            fromDate ??= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            toDate ??= fromDate.Value.AddMonths(1).AddDays(-1);

            // Filter by only date part
            query = query.Where(a =>
                a.AppoinmentDate.Date >= fromDate.Value.Date &&
                a.AppoinmentDate.Date <= toDate.Value.Date);

            return query;
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

        public int GetAppointmentsCount()
        {
            DateTime today = DateTime.Today;
            return _db.Appointments.Count(p => p.AppoinmentDate.Date == today);
        }

        public async Task<bool> AppointmentCheck(int PatientId)
        {
           return await _db.Appointments.AnyAsync(p=>p.PatientID== PatientId && p.Status==AppointmentStatus.Active || p.Status== AppointmentStatus.InProgress);
        }

        
    }

}
