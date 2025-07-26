using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class BedRepository:IBedRepository
    {
        private readonly Db _db;

        public BedRepository(Db db)
        {
            _db = db;
        }

        public List<Bed> getAllBed()
        {
           return _db.Beds.Include(d=>d.Ward).ToList();
        }

        public Bed Save(Bed bed)
        {
            _db.Beds.Add(bed);
            _db.SaveChanges();

            return bed;
        }

        public bool StatusUpdate(int id)
        {
            var bed = _db.Beds.FirstOrDefault(d => d.Id == id);
            bed.IsOccupied = !bed.IsOccupied;
            _db.SaveChanges();

            return true;
        }
    }
}
