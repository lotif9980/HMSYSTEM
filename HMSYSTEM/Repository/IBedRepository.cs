using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IBedRepository
    {
        public List<Bed> getAllBed();
        public Bed Save(Bed bed);

        public bool StatusUpdate(int id);


    }
}
