using HMSYSTEM.Data;
using HMSYSTEM.Models;

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
            //return
        }
    }
}
