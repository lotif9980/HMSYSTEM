using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IAdmissionRepository
    {
        public List<Admission> getAll();

        public int GetLastInvoiceNo();
        public void Save(Admission admission);

        public Admission GetById(int id);
        public void Delete(int id);

        public Task<bool> PatientStatusCheck(int patientId);

      
    }
}
