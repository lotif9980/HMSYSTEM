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
    }
}
