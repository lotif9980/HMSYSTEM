using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IDepartmentRepository
    {
        public List<Department> getAll();

        public List<Department> Save(Department department);
        public List<Department> Delete(int Id);
        public Department Edit(int Id);
        public Department Update(Department department);

        public Task<bool> inUsedCheck(int id);
    }
}
