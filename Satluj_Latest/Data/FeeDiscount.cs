using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class FeeDiscount : BaseReference
    {
        private TbFeeDiscount feeDiscount;
        public FeeDiscount(TbFeeDiscount obj) { feeDiscount = obj; }
        public FeeDiscount(long id) { feeDiscount = _Entities.TbFeeDiscounts.FirstOrDefault(z => z.DiscountId == id); }
        public long discountId { get { return feeDiscount.DiscountId; } }
        public long studentId { get { return feeDiscount.StudentId; } }
        public long feeId { get { return feeDiscount.FeeId; } }
        public string feename { get { return feeDiscount.Fee.FeesName ?? string.Empty; } }
        public decimal discountAmount { get { return feeDiscount.DiscountAmount; } }
        public System.DateTime Timestamp { get { return feeDiscount.TimeStamp; } }
        public bool IsActive { get { return feeDiscount.IsActive; } }
        public Student student { get { return new Student(feeDiscount.StudentId); } }

    }
}
