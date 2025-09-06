using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IScheduleRepository
    {
        IQueryable<Schedule> getAll(int?doctorId=null);
        List<Schedule> Save(Schedule schedule);
        Schedule Edit(int Id);
        Schedule Update(Schedule schedule);
        Schedule Delete(int Id);
    }
}
