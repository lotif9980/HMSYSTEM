using HMSYSTEM.Data;
using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public class ServiceItemRepository : IServiceItemRepository
    {
        protected readonly Db _db;
        public ServiceItemRepository(Db db)
        {
            _db = db;
        }

        public List<ServiceItem> GetAll()
        {
            return _db.ServiceItems.ToList();
        }
    }
}
