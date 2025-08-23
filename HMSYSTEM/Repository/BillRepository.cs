using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class BillRepository : IBillRepository
    {
        private readonly Db _db;
        
        public BillRepository(Db db)
        {
            _db = db;
        }

        public List<Bill> GetAll()
        {
          return _db.Bills
                .Include(d => d.Patient).ToList();
        }
    }
}
