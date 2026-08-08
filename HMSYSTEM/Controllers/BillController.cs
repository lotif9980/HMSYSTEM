using HMSYSTEM.Helpers;
using HMSYSTEM.Models;
using HMSYSTEM.Repository;
using HMSYSTEM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HMSYSTEM.Controllers
{
    [Authorize]
    public class BillController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BillController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var patient = _unitOfWork.PatienRepo.getAll();
            ViewBag.Patient = patient;

            var serviceItem = _unitOfWork.serviceItemRepository.GetAll();
            ViewBag.ServiceItem = serviceItem;

            var lastBillNo = _unitOfWork.billRepository
                                .GetSerial()
                                .OrderByDescending(b => b.Id)
                                .Select(b => b.BillNo)
                                .FirstOrDefault();

            string nextBillNo;

            if (!string.IsNullOrEmpty(lastBillNo))
            {
                var numberPart = lastBillNo.Substring(1);
                if (int.TryParse(numberPart, out int number))
                {
                    number++;
                    nextBillNo = "R" + number.ToString("D4");
                }
                else
                {
                    nextBillNo = "R0001";
                }
            }
            else
            {
                nextBillNo = "R0001";
            }

            ViewBag.NextSerial = nextBillNo;
            return View();
        }

        public IActionResult GetBill(string search = "", int page = 1, int pageSize = 10)
        {
            var query = _unitOfWork.billRepository.GetAll()
                                  .OrderByDescending(p => p.Id)
                                  .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(b => (b.Patient != null && (
                                            (!string.IsNullOrEmpty(b.Patient.FirstName) && b.Patient.FirstName.ToLower().Contains(search)) ||
                                            (!string.IsNullOrEmpty(b.Patient.LastName) && b.Patient.LastName.ToLower().Contains(search)) ||
                                            ((b.Patient.FirstName + " " + b.Patient.LastName).ToLower().Contains(search)) ||
                                            (!string.IsNullOrEmpty(b.Patient.Phone) && b.Patient.Phone.ToLower().Contains(search))
                                         )) ||
                                         (!string.IsNullOrEmpty(b.BillNo) && b.BillNo.ToLower().Contains(search)) ||
                                         (b.TotalAmount.HasValue && b.TotalAmount.ToString().Contains(search)) ||
                                         (b.NetAmount.HasValue && b.NetAmount.ToString().Contains(search)) ||
                                         (b.DueAmount.HasValue && b.DueAmount.ToString().Contains(search)));
            }

            var totalItem = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItem / pageSize);

            var data = query.Skip((page - 1) * pageSize).Take(pageSize).Select(b => new
            {
                id = b.Id,
                billDate = b.BillDate.ToString("dd-MM-yyyy"),
                billNo = b.BillNo,
                patientId = b.PatientId,
                patientName = b.Patient != null ? (b.Patient.FirstName + " " + b.Patient.LastName).Trim() : "",
                patientPhone = b.Patient != null ? b.Patient.Phone : "",
                totalAmount = b.TotalAmount,
                discount = b.Discount,
                netAmount = b.NetAmount,
                paymentAmt = b.PaymentAmt,
                dueAmount = b.DueAmount,
                status = b.Status,
                statusText = b.Status == 2 ? "Paid" : "Unpaid"
            }).ToList();

            return Json(new
            {
                issuccess = true,
                totalPages = totalPages,
                totalItem = totalItem,
                currentPage = page,
                data = data
            });
        }

        public IActionResult CompleteList(int page = 1, int pageSize = 10)
        {
            var data = _unitOfWork.billRepository.CompliteList()
                    .OrderByDescending(p => p.Id)
                    .AsQueryable()
                    .ToPagedList(page, pageSize);

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
                                .GetSerial()
                                .OrderByDescending(b => b.Id)
                                .Select(b => b.BillNo)
                                .FirstOrDefault();

            string nextBillNo;

            if (!string.IsNullOrEmpty(lastBillNo))
            {
                var numberPart = lastBillNo.Substring(1);
                if (int.TryParse(numberPart, out int number))
                {
                    number++;
                    nextBillNo = "R" + number.ToString("D4");
                }
                else
                {
                    nextBillNo = "R0001";
                }
            }
            else
            {
                nextBillNo = "R0001";
            }

            ViewBag.NextSerial = nextBillNo;
            return View();
        }

        [HttpPost]
        public IActionResult Save(BillViewModel model)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (model == null || model.PatientId == null)
            {
                if (isAjax) return Json(new { success = false, message = "❌ Invalid data submitted!" });
                TempData["Message"] = "❌ Invalid data!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Save");
            }

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
                BillDetails = (model.BillDetail ?? new List<BillDetailViewModel>())
                    .Where(d => d.ServiceItemId.HasValue)
                    .Select(d => new BillDetail
                    {
                        ServiceItemId = d.ServiceItemId.Value,
                        Amount = d.Amount ?? 0,
                        Qty = d.Qty ?? 0,
                        TotalAmount = d.TotalAmount ?? 0,
                        ChargeDate = d.ChargeDate
                    }).ToList()
            };

            _unitOfWork.billRepository.Save(bill);

            if (isAjax)
            {
                return Json(new { success = true, message = "✅ Bill Successfully Created!" });
            }

            TempData["Message"] = "✅ Successfully added!";
            TempData["MessageType"] = "success";

            return RedirectToAction("Save");
        }

        [HttpGet]
        public IActionResult GetBillModalData(int patientId, int bedId, int admissionId)
        {
            var patient = _unitOfWork.PatienRepo.getAll().FirstOrDefault(p => p.PatientID == patientId);
            var services = _unitOfWork.serviceItemRepository.GetAll()
                .Select(s => new { id = s.Id, itemName = s.ItemName, amount = s.Amount })
                .ToList();

            var existingBill = _unitOfWork.billRepository.GetActiveBillByPatient(patientId);

            if (existingBill == null)
            {
                return Json(new { success = false, message = "❌ Active bill not found for this patient." });
            }

            var details = (existingBill.BillDetails ?? new List<BillDetail>()).Select(d => new
            {
                id = d.Id,
                billId = d.BillId,
                serviceItemId = d.ServiceItemId,
                itemName = services.FirstOrDefault(s => s.id == d.ServiceItemId)?.itemName ?? (d.ServiceItem != null ? d.ServiceItem.ItemName : ""),
                qty = d.Qty ?? 0,
                amount = d.Amount ?? 0,
                totalAmount = d.TotalAmount ?? 0,
                chargeDate = d.ChargeDate.HasValue ? d.ChargeDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd")
            }).ToList();

            return Json(new
            {
                success = true,
                billId = existingBill.Id,
                patientId = patientId,
                patientName = patient != null ? (patient.FirstName + " " + patient.LastName).Trim() : "",
                billNo = existingBill.BillNo,
                billDate = existingBill.BillDate.ToString("yyyy-MM-dd"),
                bedId = bedId,
                admissionId = admissionId,
                totalAmount = existingBill.TotalAmount ?? 0,
                discount = existingBill.Discount ?? 0,
                netAmount = existingBill.NetAmount ?? 0,
                paymentAmt = existingBill.PaymentAmt ?? 0,
                dueAmount = existingBill.DueAmount ?? 0,
                status = existingBill.Status ?? 1,
                billDetails = details,
                services = services
            });
        }

        [HttpGet]
        public IActionResult SaveFromAdmission(int patientId, int bedId, int admissionId)
        {
            var patients = _unitOfWork.PatienRepo.getAll();
            var services = _unitOfWork.serviceItemRepository.GetAll();

            var existingBill = _unitOfWork.billRepository
                .GetActiveBillByPatient(patientId);
            if (existingBill == null)
            {
                TempData["Message"] = "❌ Data not Found";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index", "Admission");
            }
            var viewModel = new BillViewModel
            {
                Id = existingBill?.Id ?? 0,
                PatientId = patientId,
                BillNo = existingBill?.BillNo ?? "",
                BillDate = existingBill?.BillDate ?? DateTime.Now,
                TotalAmount = existingBill?.TotalAmount ?? 0,
                Discount = existingBill?.Discount ?? 0,
                NetAmount = existingBill?.NetAmount ?? 0,
                PaymentAmt = existingBill.PaymentAmt ?? 0,
                DueAmount = existingBill.DueAmount ?? 0,
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
                        TotalAmount = d.TotalAmount ?? 0,
                        ChargeDate = d.ChargeDate
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
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (model == null || model.PatientId == null)
            {
                if (isAjax) return Json(new { success = false, message = "❌ Invalid bill data!" });
                TempData["Message"] = "❌ Invalid data!";
                TempData["MessageType"] = "danger";
                return RedirectToAction("Save");
            }

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
                BillDetails = (model.BillDetail ?? new List<BillDetailViewModel>())
                    .Where(d => d.ServiceItemId.HasValue)
                    .Select(d => new BillDetail
                    {
                        ServiceItemId = d.ServiceItemId.Value,
                        Amount = d.Amount ?? 0,
                        Qty = d.Qty ?? 0,
                        TotalAmount = d.TotalAmount ?? 0,
                        ChargeDate = d.ChargeDate
                    }).ToList()
            };

            var data = _unitOfWork.billRepository.UpdateSave(bill);

            if (data.Status == 2)
            {
                _unitOfWork.bedRepository.StatusUpdate(model.BedId);
                _unitOfWork.admissionRepository.UpdateAdmissionStatus(model.AdmissionId);
            }

            if (isAjax)
            {
                return Json(new { success = true, message = "✅ Bill Successfully Saved & Updated!" });
            }

            TempData["Message"] = "✅ Successfully added!";
            TempData["MessageType"] = "success";

            return RedirectToAction("Save");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var data = _unitOfWork.billRepository.GetBillDetails(id);
            return View(data);
        }

        [HttpGet]
        public IActionResult GetBillPrintPartial(int id)
        {
            var data = _unitOfWork.billRepository.GetBillDetails(id);
            return PartialView("_PartialPrintBill", data);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            try
            {
                var bill = _unitOfWork.billRepository.GetBillDetails(id);
                if (bill == null)
                {
                    if (isAjax) return Json(new { success = false, message = "❌ Bill record not found!" });
                    TempData["Message"] = "❌ Record not found!";
                    TempData["MessageType"] = "danger";
                    return RedirectToAction("Index");
                }

                if (isAjax)
                {
                    return Json(new { success = true, message = "✅ Bill Record Processed!" });
                }
                TempData["Message"] = "✅ Successfully Processed!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "❌ " + (ex?.Message ?? "An error occurred.") });
                }
                TempData["Message"] = "❌ " + (ex?.Message ?? "An error occurred.");
                TempData["MessageType"] = "danger";
                return RedirectToAction("Index");
            }
        }
    }
}
