using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IScheduleRepository
    {
        List<Schedule> getAll();
        List<Schedule> Save(Schedule schedule);
        Schedule Edit(int Id);
        Schedule Update(Schedule schedule);
        Schedule Delete(int Id);
    }
}
