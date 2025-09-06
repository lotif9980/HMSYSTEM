using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly Db _db;

        public ScheduleRepository(Db db)
        {
            _db = db;
        }

        

        public IQueryable<Schedule> getAll(int?doctroId=null)
        {

           var query= _db.Schedules.Include(d=>d.Department)
                .Include(dr=>dr.Doctor).AsQueryable();

            if(doctroId.HasValue && doctroId.Value > 0)
            {
                query=query.Where(p=>p.DoctorId == doctroId.Value);
            }

            return query;
        }

        public List<Schedule> Save(Schedule schedule)
        {
            _db.Add(schedule);
            _db.SaveChanges();

            return _db.Schedules.ToList();
        }


        public Schedule Edit(int Id)
        {
           return _db.Schedules.FirstOrDefault(d => d.ScheduleId == Id);
        }

        public Schedule Update(Schedule schedule)
        {
            _db.Update(schedule);
            _db.SaveChanges();

            return _db.Schedules.FirstOrDefault(x => x.ScheduleId == schedule.ScheduleId);
        }

        public Schedule Delete(int Id)
        {
            var schedule = _db.Schedules.FirstOrDefault(x => x.ScheduleId == Id);
           
            if(schedule != null) 
            {
                _db.Remove(schedule);
                _db.SaveChanges();
            }
            return schedule;
        }
    }
}
