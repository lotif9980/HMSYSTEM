using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IDesignationRepository
    {
        public List<Designation> getAll();
        public Designation Find(int Id);
        public void StatusUpdate(int id);
        public List<Designation> Delete(int Id);
        public List<Designation> Update(Designation designation);
        public List<Designation> Save(Designation designation);
    }
}
