using HMSYSTEM.Data;
using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly Db _db;

        public RoleRepository(Db db)
        {
            _db = db;
        }

        public List<Role> GetRoles()
        {
            return _db.Roles.ToList();
        }
    }
}
