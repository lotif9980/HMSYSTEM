using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IDoctorRepository
    {
        public List<Doctor> getAll();
        public Doctor Details(int id);
        Doctor Edit(int Id);

        public List<Doctor> Save(Doctor doctor);
        public void Update(Doctor doctor);

        public Doctor Find(int Id);

        public void Delete(int Id);



    }
}
