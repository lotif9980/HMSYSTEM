using HMSYSTEM.Data;

namespace HMSYSTEM.Repository
{
    public class HomeRepository : IHomeRepository
    {
        private readonly Db _db;

        public HomeRepository(Db db)
        {
            _db = db;
        }
    }
}
