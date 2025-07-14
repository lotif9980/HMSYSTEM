using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IPrescriptionRepository
    {
        List<Prescription> GetAll();
        public void Save(Prescription prescription);
    }
}
