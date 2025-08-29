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
            // কত টাকা নতুন Payment এসেছে সেটা আলাদা করলাম
            decimal newPayment = bill.PaymentAmt ?? 0;

            // আগের Bill আছে কিনা দেখি
            var existingBill = _db.Bills
                .Include(b => b.BillDetails)
                .FirstOrDefault(b => b.PatientId == bill.PatientId && b.Status == 1);

            if (existingBill != null)
            {
                // 🔹 Master Fields Update
                existingBill.Discount = bill.Discount ?? 0;
                existingBill.BillDate = bill.BillDate;

                // 🔹 PaymentAmt Update
                existingBill.PaymentAmt = (existingBill.PaymentAmt ?? 0) + newPayment;

                // 🔹 BillDetails Sync
                foreach (var d in bill.BillDetails)
                {
                    var existingDetail = existingBill.BillDetails
                        .FirstOrDefault(x => x.ServiceItemId == d.ServiceItemId);

                    if (existingDetail != null)
                    {
                        existingDetail.Qty = d.Qty ?? 0;
                        existingDetail.Amount = d.Amount ?? 0;
                        existingDetail.TotalAmount = d.TotalAmount ?? 0;
                        existingDetail.ChargeDate = d.ChargeDate;
                    }
                    else
                    {
                        existingBill.BillDetails.Add(new BillDetail
                        {
                            BillId = existingBill.Id,
                            ServiceItemId = d.ServiceItemId,
                            Qty = d.Qty ?? 0,
                            Amount = d.Amount ?? 0,
                            TotalAmount = d.TotalAmount ?? 0,
                            ChargeDate = d.ChargeDate
                        });
                    }
                }

                // 🔹 Calculation
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

                // 🔹 Always Insert Payment Record
                if (newPayment > 0)
                {
                    _db.Payments.Add(new Payment
                    {
                        BillId = existingBill.Id,
                        PaymentAmount = newPayment,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "Cash",
                        CreatedDate = DateTime.Now
                    });
                }
            }
            else
            {
                // 🔹 First Time Bill Add
                _db.Bills.Add(bill);
                _db.SaveChanges(); // Save to get BillId

                // 🔹 First Payment Record
                if (newPayment > 0)
                {
                    _db.Payments.Add(new Payment
                    {
                        BillId = bill.Id,
                        PaymentAmount = newPayment,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "Cash",
                        CreatedDate = DateTime.Now
                    });
                }
            }

            _db.SaveChanges();
        }

        public Bill UpdateSave(Bill bill)
        {
            var existingBill = _db.Bills
                .Include(b => b.BillDetails)
                .FirstOrDefault(b => b.PatientId == bill.PatientId && b.Status == 1);

            if (existingBill != null)
            {
                // Original UpdateSave logic (unchanged)
                existingBill.Discount = bill.Discount ?? 0;
                existingBill.BillDate = bill.BillDate;

                foreach (var d in bill.BillDetails)
                {
                    var existingDetail = existingBill.BillDetails
                        .FirstOrDefault(x => x.ServiceItemId == d.ServiceItemId);

                    if (existingDetail != null)
                    {
                        existingDetail.Qty = d.Qty ?? 0;
                        existingDetail.Amount = d.Amount ?? 0;
                        existingDetail.TotalAmount = d.TotalAmount ?? 0;
                        existingDetail.ChargeDate = d.ChargeDate;
                    }
                    else
                    {
                        existingBill.BillDetails.Add(new BillDetail
                        {
                            BillId = existingBill.Id,
                            ServiceItemId = d.ServiceItemId,
                            Qty = d.Qty ?? 0,
                            Amount = d.Amount ?? 0,
                            TotalAmount = d.TotalAmount ?? 0,
                            ChargeDate = d.ChargeDate
                        });
                    }
                }

                // Recalculate Master Values
                decimal totalAmount = existingBill.BillDetails.Sum(d => d.TotalAmount ?? 0);
                decimal discount = existingBill.Discount ?? 0;
                decimal netAmount = totalAmount - discount;

                // Payment logic: calculate remaining/new payment
                decimal previousPayment = existingBill.PaymentAmt ?? 0;
                decimal userPayment = bill.PaymentAmt ?? 0;
                decimal paymentToInsert = userPayment - previousPayment;

                // Update Master table PaymentAmt
                existingBill.PaymentAmt = userPayment;

                decimal due = (decimal)(netAmount - existingBill.PaymentAmt);


                existingBill.TotalAmount = totalAmount;
                existingBill.NetAmount = netAmount;
                existingBill.DueAmount = due;
                existingBill.Status = (due == 0 && netAmount > 0) ? 2 : 1;

                _db.Bills.Update(existingBill);

                // Insert new Payment only if paymentToInsert > 0
                if (paymentToInsert > 0)
                {
                    _db.Payments.Add(new Payment
                    {
                        BillId = existingBill.Id,
                        PaymentAmount = paymentToInsert,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "Cash",
                        CreatedDate = DateTime.Now
                    });
                }

                _db.SaveChanges();
                return existingBill;
            }
            else
            {
                //  New Bill Add
                _db.Bills.Add(bill);
                _db.SaveChanges();

                //  Insert Payment for new bill
                if (bill.PaymentAmt > 0)
                {
                    _db.Payments.Add(new Payment
                    {
                        BillId = bill.Id,
                        PaymentAmount = bill.PaymentAmt ?? 0,
                        PaymentDate = DateTime.Now,
                        PaymentMethod = "Cash",
                        CreatedDate = DateTime.Now
                    });
                    _db.SaveChanges();
                }

                return bill;
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
