using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public PrescriptionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
           var data=  _unitOfWork.PrescriptioRepository.GetAll();
            return View(data);
        }
    }
}
