using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IBedRepository
    {
        public List<Bed> getAllBed();
        public Bed Save(Bed bed);

        public bool StatusUpdate(int id);

        public Task<bool> IsBedInUseAsync(int id);

        public List<Bed> Delete(int id);

        public Task<bool> CanAddBedToWardAsync(int id);

        public int TotalBedCount();

        public int TotalOccupied();
    }
}
