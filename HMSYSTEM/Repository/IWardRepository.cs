using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IWardRepository
    {
        public List<Ward> GetAll();
        public List<Ward> Save(Ward ward);
        public List<Ward> Delete(int id);
        public Ward Find(int id);
        public Task<bool> IsBedinUsed(int id);



      
    }
}
