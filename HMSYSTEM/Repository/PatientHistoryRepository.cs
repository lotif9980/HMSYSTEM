using HMSYSTEM.Data;
using HMSYSTEM.Models;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class PatientHistoryRepository : IPatientHistoryRepository
    {
        private readonly Db _db;

        public PatientHistoryRepository(Db db)
        {
            _db = db;
        }

        public List<PatientHistory> GetAll()
        {

          return _db.PatientHistorys
                .Include(x=>x.Patient)
                .Include(x=>x.Doctor).ToList();
        }

        public void Save(PatientHistory pHistory)
        {
            _db.Add(pHistory);
            _db.SaveChanges();
        }
    }
}
