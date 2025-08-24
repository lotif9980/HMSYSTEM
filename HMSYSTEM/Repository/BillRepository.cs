using HMSYSTEM.Data;
using HMSYSTEM.Models;
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
                .Include(d => d.Patient).ToList();
        }

        public List<Bill> GetSerial()
        {
            return _db.Bills.ToList();
        }

        public void Save(Bill bill)
        {
            var existingBill = _db.Bills
                .Include(b => b.BillDetails)
                .FirstOrDefault(b => b.PatientId == bill.PatientId && b.Status == 1);

            if (existingBill != null)
            {
                // Update Master totals
                existingBill.TotalAmount += bill.TotalAmount ?? 0;
                existingBill.Discount += bill.Discount ?? 0;
                existingBill.NetAmount += bill.NetAmount ?? 0;
                existingBill.PaymentAmt += bill.PaymentAmt ?? 0;
                existingBill.DueAmount += bill.DueAmount ?? 0;

                _db.Bills.Update(existingBill);
                _db.SaveChanges();

                // Add only new Child Details (prevent duplicate)
                if (bill.BillDetails != null && bill.BillDetails.Count > 0)
                {
                    var newDetails = bill.BillDetails
                        .Where(d => !existingBill.BillDetails.Any(ed => ed.ServiceItemId == d.ServiceItemId))
                        .Select(d => new BillDetail
                        {
                            BillId = existingBill.Id,
                            ServiceItemId = d.ServiceItemId,
                            Amount = d.Amount ?? 0,
                            Qty = d.Qty ?? 0,
                            TotalAmount = d.TotalAmount ?? 0
                        }).ToList();

                    if (newDetails.Any())
                    {
                        _db.BillDetails.AddRange(newDetails);
                        _db.SaveChanges();
                    }
                }
            }
            else
            {
                // New Master Bill
                // Just add Bill with its attached BillDetails
                _db.Bills.Add(bill);
                _db.SaveChanges(); // EF Core will insert child automatically
            }
        }

    }
}
