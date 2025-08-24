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

        [HttpGet]
        public IActionResult Save()
        {
            var patient=_unitOfWork.PatienRepo.getAll();
            ViewBag.Patient = patient;
            var serviceItem=_unitOfWork.serviceItemRepository.GetAll();
            ViewBag.ServiceItem = serviceItem;
            string nextBillNo = "R000"; 
            if (!string.IsNullOrEmpty(nextBillNo))
            {
                
                var numberPart = nextBillNo.Substring(1); 
                int number;
                if (int.TryParse(numberPart, out number))
                {
                    number += 1; 
                    nextBillNo = "R" + number.ToString("D4");
                }
            }
            ViewBag.NextSerial = nextBillNo;
            return View();
        }
    }
}
