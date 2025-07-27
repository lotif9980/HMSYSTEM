using HMSYSTEM.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HMSYSTEM.Controllers
{
    public class AdmissionController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdmissionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var data = _unitOfWork.admissionRepository.getAll();
            return View(data);
        }

        public IActionResult Save()
        {

            int lastSerial = _unitOfWork.admissionRepository.GetLastInvoiceNo();
            int nextSerial = lastSerial + 1;
            ViewBag.NextSerial = nextSerial;


            return View();
        }
    }
}
