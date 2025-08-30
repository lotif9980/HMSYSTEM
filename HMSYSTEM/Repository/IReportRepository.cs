using HMSYSTEM.ViewModels;

namespace HMSYSTEM.Repository
{
    public interface IReportRepository
    {
        public List<WardBedViewModel> GetWardBedStatus(int?wardId=null);
    }
}
