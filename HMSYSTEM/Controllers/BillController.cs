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

        public IActionResult Index()
        {
            return View();
        }
    }
}
