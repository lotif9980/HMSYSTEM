using HMSYSTEM.Data;
using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IBillRepository
    {
        public List<Bill> GetAll();
        public List<Bill> GetSerial();
        public void Save(Bill bill);
        public Bill GetActiveBillByPatient(int patientId);
        
    }
}
