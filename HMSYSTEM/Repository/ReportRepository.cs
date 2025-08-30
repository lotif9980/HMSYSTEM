using AspNetCoreGeneratedDocument;
using HMSYSTEM.Data;
using HMSYSTEM.Models;
using HMSYSTEM.ViewModels;

namespace HMSYSTEM.Repository
{
    public class ReportRepository : IReportRepository
    {
        private protected readonly Db _db;

        public ReportRepository(Db db)
        {
            _db = db;
        }

        public List<WardBedViewModel> GetWardBedStatus(int? wardId = null)
        {
            var data = from w in _db.Wards
                       where !wardId.HasValue || w.Id == wardId.Value
                       from b in _db.Beds.Where(b => b.WardId == w.Id).DefaultIfEmpty()
                       select new WardBedViewModel
                       {
                           WardName = w.Name,
                           BedNumber = b != null ? b.BedNumber : "-",
                           IsOccupied = b != null && b.IsOccupied != null // patient info লাগছে না
                       };

            return data.ToList();
        }
    }
}
