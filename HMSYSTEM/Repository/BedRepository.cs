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
    }
}
