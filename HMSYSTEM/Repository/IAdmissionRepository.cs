using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IAdmissionRepository
    {
        public List<Admission> getAll();

        public int GetLastInvoiceNo();
        public void Save(Admission admission);
    }
}
