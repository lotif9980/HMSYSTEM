using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IServiceItemRepository
    {
        public List<ServiceItem> GetAll();

        public ServiceItem Save(ServiceItem item);
        public ServiceItem Find(int id);
        public void Update(ServiceItem item);
        public void Delete(int id);
    }
}
