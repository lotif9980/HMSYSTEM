using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IPrescriptionRepository
    {
        List<Prescription> GetAll();
    }
}
