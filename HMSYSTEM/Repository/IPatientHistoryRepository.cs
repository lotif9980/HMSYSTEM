using HMSYSTEM.Models;

namespace HMSYSTEM.Repository
{
    public interface IPatientHistoryRepository
    {

        public List<PatientHistory> GetAll();
        public void Save(PatientHistory pHistory);

      
    }
}
