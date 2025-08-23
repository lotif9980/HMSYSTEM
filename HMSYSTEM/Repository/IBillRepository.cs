using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IBillRepository
    {
        public List<Bill> GetAll();
    }
}
