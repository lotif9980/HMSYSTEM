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

        public IActionResult GetPatientPhoneNumber(string PhoneNumber)
        {
          var phoneNumber= _unitOfWork.PatienRepo.getAll().FirstOrDefault(p=>p.Phone==PhoneNumber && p.Status==true);
            return View();
        
        }

    }
}
