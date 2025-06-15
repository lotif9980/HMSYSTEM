using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Repository
{
    public class DesignationRepository : IDesignationRepository
    {
        private readonly Db _db;

        public DesignationRepository(Db db)
        {
            _db = db;
        }

        

        public List<Designation> getAll()
        {
            return _db.Designations.ToList();
        }

        public Designation Find(int Id)
        {
            return _db.Designations.Find(Id);
        }

        public List<Designation> Delete(int Id)
        {
           var data=_db.Designations.Find(Id);
            _db.Remove(data);
            _db.SaveChanges();

            return _db.Designations.ToList();
        }

        [HttpPut]
        public List<Designation> Update(Designation designation)
        {
            _db.Update(designation);
            _db.SaveChanges();

            return _db.Designations.ToList();
        }

        public List<Designation> Save(Designation designation)
        {
            _db.Add(designation);
            _db.SaveChanges();

            return _db.Designations.ToList();
        }
    }
}
