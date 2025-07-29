using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
           return _db.Beds.Include(d=>d.Ward).OrderBy(d=>d.Id).ToList();
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

        public Task<bool> IsBedInUseAsync(int id)
        {
            return  _db.Admissions.AnyAsync(a => a.BedId == id);
        }

        public List <Bed> Delete(int id)
        {
            var data = _db.Beds.Find(id);
            _db.Remove(data);
            _db.SaveChanges();

            return _db.Beds.ToList();
        }

        public async Task<bool> CanAddBedToWardAsync(int id)
        {
            var ward =await _db.Wards.FirstOrDefaultAsync(d=>d.Id == id);
            var exestingBed=await _db.Beds.CountAsync(d=>d.WardId == id);

            return exestingBed < ward.TotalBeds;
        }
    }
}
