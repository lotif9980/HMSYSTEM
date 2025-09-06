using HMSYSTEM.Models;
using HMSYSTEM.ViewModels;

namespace HMSYSTEM.Repository
{
    public interface IPrescriptionRepository
    {
        IQueryable<Prescription> GetAll(int?doctorId=null);
        public void Save(Prescription prescription);

        public void Delete(int id);

        public PrescriptionViewModel GetPrescriptionViewModel(int id);

        int GetCountPrescription();

        public Prescription Find(int id);
    }
}
