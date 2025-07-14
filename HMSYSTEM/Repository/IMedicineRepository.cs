using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IMedicineRepository
    {

        List<Medicine> GetAllMedicines();
        public void Save(Medicine medicine);

    }
}
