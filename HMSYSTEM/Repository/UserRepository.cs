using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class UserRepository:IUserRepository
    {
        protected readonly Db _db;
       

        public UserRepository(Db db)
        {
            _db = db;
        }

        public List<User> GetAll()
        {  
            return _db.Users.ToList(); 
        }

        public User GetUser(string username, string password)
        {
            return _db.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
        }

        List<User> IUserRepository.Save(User user)
        {
            _db.Users.Add(user);
            _db.SaveChanges();

            return _db.Users.ToList();
        }
    }
}
