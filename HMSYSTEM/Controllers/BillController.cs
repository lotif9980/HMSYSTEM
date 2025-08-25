using HMSYSTEM.Helpers;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
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
         
            var patient = _unitOfWork.PatienRepo.getAll();
            ViewBag.Patient = patient;

      
            var serviceItem = _unitOfWork.serviceItemRepository.GetAll();
            ViewBag.ServiceItem = serviceItem;

          
            var lastBillNo = _unitOfWork.billRepository
                                .GetAll() // বা নির্দিষ্ট মেথড, যেমন: GetSerial()
                                .OrderByDescending(b => b.Id)
                                .Select(b => b.BillNo)
                                .FirstOrDefault();

            string nextBillNo;

            if (!string.IsNullOrEmpty(lastBillNo))
            {
                // উদাহরণ: R0001 → 0001 নিয়ে সংখ্যা বের করা
                var numberPart = lastBillNo.Substring(1); // R বাদ দিয়ে
                if (int.TryParse(numberPart, out int number))
                {
                    number++;
                    nextBillNo = "R" + number.ToString("D4"); // R0002 এর মত
                }
                else
                {
                    nextBillNo = "R0001"; // ফেইল হলে শুরু থেকে
                }
            }
            else
            {
                nextBillNo = "R0001"; // যদি কিছুই না থাকে
            }

            ViewBag.NextSerial = nextBillNo;
            return View();
        }

        #region old version Save Method
        //[HttpPost]
        //public IActionResult Save(BillViewModel model)
        //{
        //    var bill = new Bill
        //    {
        //        BillNo=model.BillNo,
        //        BillDate=model.BillDate,
        //        PatientId=model.PatientId,
        //        TotalAmount=model.TotalAmount,
        //        Discount=model.Discount,
        //        NetAmount=model.NetAmount,
        //        PaymentAmt=model.PaymentAmt,
        //        DueAmount=model.DueAmount,
        //        Note=model.Note
        //    };

        //    if(model.BillDetail != null && model.BillDetail.Count >0)
        //    {
        //        bill.BillDetails = model.BillDetail
        //        .Where(d => d.ServiceItemId.HasValue)
        //        .Select(d => new BillDetail
        //        {
        //            ServiceItemId=d.ServiceItemId.Value,
        //            Amount=d.Amount,
        //            Qty=d.Qty,
        //            TotalAmount=d.TotalAmount,
        //        }).ToList();
        //    }

        //    _unitOfWork.billRepository.Save(bill);
        //    TempData["Message"] = "✅ Successfully added!";
        //    TempData["MessageType"] = "success";

        //    return RedirectToAction("Save");
        //}

        #endregion


        [HttpPost]
        public IActionResult Save(BillViewModel model)
        {
            if (model == null || model.PatientId == null)
            {
                TempData["Message"] = "❌ Invalid data!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Save");
            }

            // Map ViewModel to Entity
            var bill = new Bill
            {
                BillNo = model.BillNo,
                BillDate = model.BillDate,
                PatientId = model.PatientId,
                TotalAmount = model.TotalAmount ?? 0,
                Discount = model.Discount ?? 0,
                NetAmount = model.NetAmount ?? 0,
                PaymentAmt = model.PaymentAmt ?? 0,
                DueAmount = model.DueAmount ?? 0,
                Note = model.Note,
                Status = 1,
                BillDetails = model.BillDetail
                    .Where(d => d.ServiceItemId.HasValue)
                    .Select(d => new BillDetail
                    {
                        ServiceItemId = d.ServiceItemId.Value,
                        Amount = d.Amount ?? 0,
                        Qty = d.Qty ?? 0,
                        TotalAmount = d.TotalAmount ?? 0
                        // Do NOT assign Id
                    }).ToList()
            };

            _unitOfWork.billRepository.Save(bill);

            TempData["Message"] = "✅ Successfully added!";
            TempData["MessageType"] = "success";

            return RedirectToAction("Save");
        }

        public IActionResult SaveFromAdmission(int patientId)
        {
            // 1️⃣ Patient ও ServiceItem ViewBag এ পাঠানো
            ViewBag.Patient = _unitOfWork.PatienRepo.getAll();
            ViewBag.ServiceItem = _unitOfWork.serviceItemRepository.GetAll();

            // 2️⃣ আগের Bill check
            var existingBill = _unitOfWork.billRepository
                                .GetActiveBillByPatient(patientId);

            BillViewModel model;

            if (existingBill != null)
            {
                // আগের Bill load
                model = new BillViewModel
                {
                    Id = existingBill.Id,
                    BillNo = existingBill.BillNo,      // নতুন BillNo generate করার দরকার নেই
                    BillDate = existingBill.BillDate,
                    PatientId = existingBill.PatientId,
                    TotalAmount = existingBill.TotalAmount,
                    Discount = existingBill.Discount,
                    NetAmount = existingBill.NetAmount,
                    PaymentAmt = existingBill.PaymentAmt,
                    DueAmount = existingBill.DueAmount,
                    Note = existingBill.Note,
                    BillDetail = existingBill.BillDetails.Select(d => new BillDetailViewModel
                    {
                        Id = d.Id,
                        BillId = d.BillId,
                        ServiceItemId = d.ServiceItemId,
                        Qty = d.Qty ?? 0,
                        Amount = d.Amount ?? 0,
                        TotalAmount = d.TotalAmount ?? 0
                    }).ToList()
                };
            }
            else
            {
                // শুধুমাত্র যদি কোন Bill না থাকে
                model = new BillViewModel
                {
                    BillDate = DateTime.Now,
                    PatientId = patientId
                };
            }

            return View("Save", model); // Save view ব্যবহার হবে
        }



        [HttpPost]
        public IActionResult Savea(BillViewModel model)
        {
            if (model == null || model.PatientId == null)
                return RedirectToAction("Save");

            var bill = new Bill
            {
                BillNo = model.BillNo,
                BillDate = model.BillDate,
                PatientId = model.PatientId,
                TotalAmount = model.TotalAmount ?? 0,
                Discount = model.Discount ?? 0,
                NetAmount = model.NetAmount ?? 0,
                PaymentAmt = model.PaymentAmt ?? 0,
                DueAmount = model.DueAmount ?? 0,
                Note = model.Note,
                Status = 1,
                BillDetails = model.BillDetail
                    .Where(d => d.ServiceItemId.HasValue)
                    .Select(d => new BillDetail
                    {
                        ServiceItemId = d.ServiceItemId.Value,
                        Qty = d.Qty ?? 0,
                        Amount = d.Amount ?? 0,
                        TotalAmount = d.TotalAmount ?? 0
                    }).ToList()
            };

            _unitOfWork.billRepository.Save(bill);

            return RedirectToAction("Save");
        }
    }
}
