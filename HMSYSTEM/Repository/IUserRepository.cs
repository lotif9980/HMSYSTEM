using HMSYSTEM.Data;
using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User GetUser(string username, string password);

        List<User> Save(User user);
        void Delete(int Id);
        User Find(int Id);

        public void Update(User user);

        public void GetStatus(int id);
     

    }
}
