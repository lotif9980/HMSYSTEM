using HMSYSTEM.Data;
using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly Db _db;

        public DepartmentRepository(Db db)
        {
            _db = db;
        }

        

        public List<Department> getAll()
        {
            
            return _db.Departments.ToList();
        }

        public List<Department> Delete(int Id)
        {
            var data = _db.Departments.Find(Id);
            _db.Remove(data);
            _db.SaveChanges();

            return _db.Departments.ToList();
        }

        public Department Edit(int Id)
        {
          return  _db.Departments.Find(Id);
        }

        public List<Department> Save(Department department)
        {
            _db.Add(department);
            _db.SaveChanges();

            return _db.Departments.ToList();
        }

        public Department Update (Department department)
        {
            _db.Update(department);
            _db.SaveChanges();

            return _db.Departments.Where(x=>x.DepartmentId==department.DepartmentId).FirstOrDefault();
        }
    }
}
