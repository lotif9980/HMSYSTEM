using HMSYSTEM.Helpers;
using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class BillController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BillController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(int page=1, int pageSize=10)
        {
           var data =_unitOfWork.billRepository.GetAll()
                    .OrderBy(p=>p.Id)
                    .AsQueryable()
                    .ToPagedList(page,pageSize);

            return View(data);
        }
    }
}
