using HMSYSTEM.Data;
using HMSYSTEM.Models;
using HMSYSTEM.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HMSYSTEM.Repository
{
    public class BillRepository : IBillRepository
    {
        private readonly Db _db;
        
        public BillRepository(Db db)
        {
            _db = db;
        }

        public List<Bill> GetAll()
        {
          return _db.Bills
                .Include(d => d.Patient).Where(p=>p.Status==1).ToList();
        }

        public List<Bill> CompliteList()
        {
            return _db.Bills
                   .Include(d => d.Patient).Where(p => p.Status == 2).ToList();
        }

        public List<Bill> GetSerial()
        {
            return _db.Bills.ToList();
        }

        #region oldSave
        //public void Save(Bill bill)
        //{
        //    var existingBill = _db.Bills
        //        .Include(b => b.BillDetails)
        //        .FirstOrDefault(b => b.PatientId == bill.PatientId && b.Status == 1);

        //    if (existingBill != null)
        //    {
        //        // Update Master totals
        //        existingBill.TotalAmount += bill.TotalAmount ?? 0;
        //        existingBill.Discount += bill.Discount ?? 0;
        //        existingBill.NetAmount += bill.NetAmount ?? 0;
        //        existingBill.PaymentAmt += bill.PaymentAmt ?? 0;
        //        existingBill.DueAmount += bill.DueAmount ?? 0;

        //        _db.Bills.Update(existingBill);
        //        _db.SaveChanges();

        //        // Add only new Child Details (prevent duplicate)
        //        if (bill.BillDetails != null && bill.BillDetails.Count > 0)
        //        {
        //            var newDetails = bill.BillDetails
        //                .Where(d => !existingBill.BillDetails.Any(ed => ed.ServiceItemId == d.ServiceItemId))
        //                .Select(d => new BillDetail
        //                {
        //                    BillId = existingBill.Id,
        //                    ServiceItemId = d.ServiceItemId,
        //                    Amount = d.Amount ?? 0,
        //                    Qty = d.Qty ?? 0,
        //                    TotalAmount = d.TotalAmount ?? 0
        //                }).ToList();

        //            if (newDetails.Any())
        //            {
        //                _db.BillDetails.AddRange(newDetails);
        //                _db.SaveChanges();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // New Master Bill
        //        // Just add Bill with its attached BillDetails
        //        _db.Bills.Add(bill);
        //        _db.SaveChanges(); // EF Core will insert child automatically
        //    }
        //}

        #endregion
        public void Save(Bill bill)
            {
            var existingBill = _db.Bills
                .Include(b => b.BillDetails)
                .FirstOrDefault(b => b.PatientId == bill.PatientId && b.Status == 1);

            if (existingBill != null)
            {
                // ✅ Master Fields Update
                existingBill.Discount = bill.Discount ?? 0;
                existingBill.BillDate = bill.BillDate;

                // ✅ PaymentAmt (Add old + new)
                existingBill.PaymentAmt = (existingBill.PaymentAmt ?? 0) + (bill.PaymentAmt ?? 0);

                // ✅ Sync BillDetails (Add/Update Only)
                foreach (var d in bill.BillDetails)
                {
                    var existingDetail = existingBill.BillDetails
                        .FirstOrDefault(x => x.ServiceItemId == d.ServiceItemId);

                    if (existingDetail != null)
                    {
                        // Update existing detail
                        existingDetail.Qty = d.Qty ?? 0;
                        existingDetail.Amount = d.Amount ?? 0;
                        existingDetail.TotalAmount = d.TotalAmount ?? 0;
                    }
                    else
                    {
                        // Add new detail
                        existingBill.BillDetails.Add(new BillDetail
                        {
                            BillId = existingBill.Id,
                            ServiceItemId = d.ServiceItemId,
                            Qty = d.Qty ?? 0,
                            Amount = d.Amount ?? 0,
                            TotalAmount = d.TotalAmount ?? 0,
                            ChargeDate=d.ChargeDate
                        });
                    }
                }

                // ✅ Calculation from BillDetails
                decimal totalAmount = existingBill.BillDetails.Sum(d => d.TotalAmount ?? 0);
                decimal discount = existingBill.Discount ?? 0;
                decimal netAmount = totalAmount - discount;
                decimal payment = existingBill.PaymentAmt ?? 0;
                decimal due = netAmount - payment;

                // ✅ Update Master table values
                existingBill.TotalAmount = totalAmount;
                existingBill.NetAmount = netAmount;
                existingBill.DueAmount = due;

                // ✅ Status Update
                if (due == 0 && netAmount > 0)
                    existingBill.Status = 2; // Fully Paid
                else
                    existingBill.Status = 1; // Active/Unpaid

                _db.Bills.Update(existingBill);
            }
            else
            {
                // ✅ New Bill Add
                _db.Bills.Add(bill);
            }

            _db.SaveChanges();
        }

        public Bill UpdateSave(Bill bill)
        {
            // 🔹 Check if there is an existing active bill for the patient
            var existingBill = _db.Bills
                .Include(b => b.BillDetails)
                .FirstOrDefault(b => b.PatientId == bill.PatientId && b.Status == 1);

            if (existingBill != null)
            {
                // 🔹 Update Master Fields
                existingBill.Discount = bill.Discount ?? 0;
                existingBill.PaymentAmt = bill.PaymentAmt ?? 0; // ⚠️ Replace or accumulate if needed
                existingBill.BillDate = bill.BillDate;

                // 🔹 Sync BillDetails (Add or Update)
                foreach (var d in bill.BillDetails)
                {
                    var existingDetail = existingBill.BillDetails
                        .FirstOrDefault(x => x.ServiceItemId == d.ServiceItemId);

                    if (existingDetail != null)
                    {
                        // Update existing detail
                        existingDetail.Qty = d.Qty ?? 0;
                        existingDetail.Amount = d.Amount ?? 0;
                        existingDetail.TotalAmount = d.TotalAmount ?? 0;
                        existingDetail.ChargeDate = d.ChargeDate;
                    }
                    else
                    {
                        // Add new detail
                        existingBill.BillDetails.Add(new BillDetail
                        {
                            BillId = existingBill.Id,
                            ServiceItemId = d.ServiceItemId,
                            Qty = d.Qty ?? 0,
                            Amount = d.Amount ?? 0,
                            TotalAmount = d.TotalAmount ?? 0,
                            ChargeDate=d.ChargeDate
                        });
                    }
                }

                // 🔹 Recalculate Master Values from Child
                decimal totalAmount = existingBill.BillDetails.Sum(d => d.TotalAmount ?? 0);
                decimal discount = existingBill.Discount ?? 0;
                decimal netAmount = totalAmount - discount;
                decimal payment = existingBill.PaymentAmt ?? 0;
                decimal due = netAmount - payment;

                existingBill.TotalAmount = totalAmount;
                existingBill.NetAmount = netAmount;
                existingBill.DueAmount = due;

                // 🔹 Status Update
                existingBill.Status = (due == 0 && netAmount > 0) ? 2 : 1;

                _db.Bills.Update(existingBill);

                _db.SaveChanges();

                return existingBill; // ✅ Return updated Bill
            }
            else
            {
                // 🔹 New Bill Add
                _db.Bills.Add(bill);
                _db.SaveChanges();
                return bill; // ✅ Return new Bill
            }
        }




        public Bill GetActiveBillByPatient(int patientId)
        {
            return _db.Bills
                .Include(b => b.BillDetails) 
                .FirstOrDefault(b => b.PatientId == patientId && b.Status == 1);
        }
        
        public BillViewModel? GetBillDetails(int id)
        {
            var viewModel = (from b in _db.Bills
                             join p in _db.Patients on b.PatientId equals p.PatientID into pgroup 
                             from p in pgroup.DefaultIfEmpty()
                             where b.Id == id
                             select new BillViewModel 
                             { 
                                 Id=b.Id,
                                 PatientId=b.PatientId,
                                 //PatientName=b.Patient.FirstName +""+b.Patient.LastName,
                                 PatientName = p != null ? p.FirstName + " " + p.LastName : null,
                                 PatientPhoneNumber =b.Patient.Phone,
                                 Address=p.Address,
                                 BillDate=b.BillDate,
                                 BillNo=b.BillNo,
                                 TotalAmount=b.TotalAmount,
                                 Discount=b.Discount,
                                 NetAmount=b.NetAmount,
                                 PaymentAmt=b.PaymentAmt,
                                 DueAmount=b.DueAmount,
                                 Status=b.Status
                             }).FirstOrDefault();

            viewModel.BillDetail = (from bd in _db.BillDetails
                                    join si in _db.ServiceItems on bd.ServiceItemId equals si.Id into sgroup
                                    from si in sgroup.DefaultIfEmpty()
                                    where bd.BillId==id
                                    select new BillDetailViewModel
                                    {
                                        
                                        Id=bd.Id,
                                        BillId = bd.BillId,
                                        ServiceItemId =si.Id,
                                        //ItemName=si.ItemName,
                                        ItemName = si != null ? si.ItemName : null,
                                        Qty =bd.Qty,
                                        Amount=bd.Amount,
                                        TotalAmount=bd.TotalAmount,
                                        ChargeDate=bd.ChargeDate

                                    }).ToList();

            return viewModel;
        }
    }
}
