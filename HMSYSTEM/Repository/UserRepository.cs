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

        //public User GetUser(string username, string password)
        //{
        //    return _db.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
        //}


        public User GetUser(string username, string password)
        {
         
            var user = _db.Users.FirstOrDefault(u => u.UserName == username);

            if (user != null)
            {
                
                var hashedPassword = user.Password;

             
                bool isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

                if (isValid)
                {
                   
                    return user;
                }
            }

         
            return null;
        }

        public List<User> Save(User user)
        {
            
            bool exists = _db.Users.Any(u => u.UserName == user.UserName);
            if (exists)
            {
                
                throw new Exception("Username already exists. Please choose another.");
            }

           
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

          
            _db.Users.Add(user);
            _db.SaveChanges();

            return _db.Users.ToList();
        }

       
    }
}
