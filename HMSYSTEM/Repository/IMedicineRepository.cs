using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IMedicineRepository
    {

        List<Medicine> GetAllMedicines();
        public void Save(Medicine medicine);
        public void Delete(int Id);
        public void Update(Medicine medicine);
        public Medicine Find(int Id);



    }
}
