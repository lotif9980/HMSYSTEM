using HMSYSTEM.Data;

namespace HMSYSTEM.Repository
{
    public class BillRepository : IBillRepository
    {
        private readonly Db _db;
        
        public BillRepository(Db db)
        {
            _db = db;
        }


    }
}
