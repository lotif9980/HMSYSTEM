using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IWardRepository
    {
        public List<Ward> GetAll();
        public List<Ward> Save(Ward ward);
        public void Delete(int id);
        public Ward Find(int id);
    }
}
