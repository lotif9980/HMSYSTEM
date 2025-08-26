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
            var patients = _unitOfWork.PatienRepo.getAll();
            var services = _unitOfWork.serviceItemRepository.GetAll();

            var existingBill = _unitOfWork.billRepository
                .GetActiveBillByPatient(patientId); // এখানে Include দিয়ে BillDetails আনতে হবে

            var viewModel = new BillViewModel
            {
                Id = existingBill?.Id ?? 0,
                PatientId = patientId,
                BillNo = existingBill?.BillNo ?? "",
                BillDate = existingBill?.BillDate ?? DateTime.Now,
                TotalAmount = existingBill?.TotalAmount ?? 0,
                Discount = existingBill?.Discount ?? 0,
                NetAmount = existingBill?.NetAmount ?? 0,
                PaymentAmt=existingBill.PaymentAmt ?? 0,
                DueAmount=existingBill.DueAmount ?? 0,
                Status = existingBill?.Status ?? 1,
                BillDetail = new List<BillDetailViewModel>()
            };

            if (existingBill != null && existingBill.BillDetails != null)
            {
                foreach (var d in existingBill.BillDetails)
                {
                    viewModel.BillDetail.Add(new BillDetailViewModel
                    {
                        Id = d.Id,
                        BillId = d.BillId,
                        ServiceItemId = d.ServiceItemId,
                        Qty = d.Qty ?? 0,
                        Amount = d.Amount ?? 0,
                        TotalAmount = d.TotalAmount ?? 0
                    });
                }
            }

            ViewBag.Patient = patients;
            ViewBag.ServiceItem = services;

            return View(viewModel);
        }




  
        [HttpPost]
        public IActionResult UpdateSave(BillViewModel model)
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

            _unitOfWork.billRepository.UpdateSave(bill);

            if (bill.Status == 2)
            {
               // code uncomplete 
            }

            TempData["Message"] = "✅ Successfully added!";
            TempData["MessageType"] = "success";

            return RedirectToAction("Save");
        }
    }
}
