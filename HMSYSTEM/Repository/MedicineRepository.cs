using HMSYSTEM.Data;
using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public class MedicineRepository:IMedicineRepository
    {
        private readonly Db _db;

        public MedicineRepository(Db db)
        {
            _db = db;
        }

        public List<Medicine> GetAllMedicines()
        {
           return _db.Medicines.ToList();
        }

        public void Save(Medicine medicine)
        {
            _db.Add(medicine);
            _db.SaveChanges();
        }

        public void Delete(int Id)
        {
            var data=_db.Medicines.Find(Id);
            _db.Remove(data);
            _db.SaveChanges();
        }
    }
}
