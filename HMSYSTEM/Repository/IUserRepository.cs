using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User GetUser(string username, string password);
       
    }
}
