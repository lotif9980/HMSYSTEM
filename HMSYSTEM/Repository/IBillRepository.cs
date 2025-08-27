using HMSYSTEM.Data;
using HMSYSTEM.Models;
using HMSYSTEM.ViewModels;

namespace HMSYSTEM.Repository
{
    public interface IBillRepository
    {
        public List<Bill> GetAll();
   
        public List<Bill> CompliteList();
        public List<Bill> GetSerial();
        public void Save(Bill bill);
        //public void UpdateSave(Bill bill);
        public Bill GetActiveBillByPatient(int patientId);
        public Bill UpdateSave(Bill bill);

        public BillViewModel GetBillDetails(int id);
    }
}
