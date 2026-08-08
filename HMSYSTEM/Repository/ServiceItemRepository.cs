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

        public ServiceItem Save(ServiceItem item)
        {
            _db.ServiceItems.Add(item);
            _db.SaveChanges();
            return item;
        }

        public ServiceItem Find(int id)
        {
            return _db.ServiceItems.Find(id);
        }

        public void Update(ServiceItem item)
        {
            var existing = _db.ServiceItems.Find(item.Id);
            if (existing != null)
            {
                existing.ItemName = item.ItemName;
                existing.Amount = item.Amount;
                existing.IsActive = item.IsActive;
                _db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            var data = _db.ServiceItems.Find(id);
            if (data != null)
            {
                _db.ServiceItems.Remove(data);
                _db.SaveChanges();
            }
        }
    }
}
