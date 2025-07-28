using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class AdmissionRepository : IAdmissionRepository
    {
        private readonly Db _db;
        public AdmissionRepository(Db db)
        {
            _db = db;
        }

        public List<Admission> getAll()
        {
          //return _db.Admissions.ToList();
          return _db.Admissions.Include(d=>d.Patient)
                .Include(d=>d.Doctor)
                .Include(d=>d.Bed)
                .ThenInclude(b=>b.Ward).ToList();
        }

        public int GetLastInvoiceNo()
        {
            return _db.Admissions
                .OrderByDescending(a => a.InvoiceNo)
                .Select(a => a.InvoiceNo)
                .FirstOrDefault();
        }

        public void Save(Admission admission)
        {
            _db.Add(admission);
            _db.SaveChanges();
        }

        public void Delete(int id)
        {
           var data= _db.Admissions.Find(id);
            _db.Remove(data);
            _db.SaveChanges();
        }

        public Admission GetById(int id)
        {
            return _db.Admissions
                .Include(p=>p.Patient)
                .Include(p=>p.Doctor)
                .Include(p=>p.Bed)
                .ThenInclude(p=>p.Ward)
                .ThenInclude(p=>p.Department)
                .FirstOrDefault(a=>a.Id==id);
        }

    }
}
