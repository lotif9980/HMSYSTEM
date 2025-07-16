using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class PrescriptionRepository:IPrescriptionRepository
    {
        private readonly Db _db;


        public PrescriptionRepository(Db db)
        {
            _db = db;
        }

      

        public List<Prescription> GetAll()
        {
            return _db.Prescriptions
                .Include(d=>d.Doctor)
                .Include(d=>d.Department)
                .Include(p=>p.Patient).ToList();
        }

        public void Save(Prescription prescription)
        {
            _db.Prescriptions.Add(prescription);
            _db.SaveChanges();
        }

        public void  Delete(int id)
        {
            //var data =_db.Prescriptions.Find(id);
            var data=_db.Prescriptions.Include(p=>p.PrescriptionDetails).FirstOrDefault(p=>p.Id == id);

            _db.PrescriptionDetails.RemoveRange(data.PrescriptionDetails);
            _db.Prescriptions.Remove(data);
            _db.SaveChanges();
        }
    }
}
