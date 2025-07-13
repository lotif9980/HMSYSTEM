using HMSYSTEM.Data;
using HMSYSTEM.Models;

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
          return _db.PatientHistories.ToList();
        }
    }
}
