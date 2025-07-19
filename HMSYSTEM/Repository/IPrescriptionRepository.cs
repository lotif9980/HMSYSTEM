using HMSYSTEM.Models;
using HMSYSTEM.ViewModels;

namespace HMSYSTEM.Repository
{
    public interface IPrescriptionRepository
    {
        List<Prescription> GetAll();
        public void Save(Prescription prescription);

        public void Delete(int id);

        public PrescriptionViewModel GetPrescriptionViewModel(int id);
    }
}
