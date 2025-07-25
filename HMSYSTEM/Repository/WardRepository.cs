using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class WardRepository:IWardRepository
    {
        private readonly Db _db;
        
        public WardRepository(Db db)
        {
            _db = db;
        }

        public List<Ward> GetAll()
        {
            return _db.Wards.Include(d=>d.Department).ToList();
        }

        public List<Ward> Save(Ward ward)
        {
            _db.Add(ward);
            _db.SaveChanges();
            return _db.Wards.ToList();
        }

        public void Delete(int id)
        {
           var data= _db.Wards.Find(id);

            _db.Remove(data);
            _db.SaveChanges();
        }

        public Ward Find(int id)
        {
            return _db.Wards.Include(d=>d.Department).FirstOrDefault(d=>d.Id==id);
        }
        
    }
}
