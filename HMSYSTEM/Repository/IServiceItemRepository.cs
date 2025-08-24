using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IServiceItemRepository
    {
        public List<ServiceItem> GetAll();

        public ServiceItem Save(ServiceItem item);
    }
}
