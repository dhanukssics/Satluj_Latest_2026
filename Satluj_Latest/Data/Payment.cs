using Satluj_Latest.Models;


namespace Satluj_Latest.Data
{
    public class Payment : BaseReference
    {
        private TbPayment payment;
        public Payment(TbPayment obj) { payment = obj; }
        public Payment(long id) { payment = _Entities.TbPayments.FirstOrDefault(z => z.PaymentId == id); }
        public System.Guid FeeGuid { get { return payment.FeeGuid; } }
        public long PaymentId { get { return payment.PaymentId; } }
        public decimal Amount { get { return payment.Amount; } }
        public decimal? Discount { get { return payment.Discount; } }
        public decimal? MaxAmount { get { return payment.MaxAmount; } }
        public System.Guid? PaymentGuid { get { return payment.PaymentGuid; } }
        public long? BillNo { get { return payment.BillNo; } }
        public bool IsPaid { get { return payment.IsPaid; } }
        public long FeeId { get { return payment.FeeId; } }
        public long StudentId { get { return payment.StudentId; } }
        public long ClassId { get { return payment.ClassId; } }
        public long SchoolId { get { return payment.ClassId; } }
        public System.DateTime TimeStamp { get { return payment.TimeStamp; } }
        public bool IsActive { get { return payment.IsActive; } }
        public TbFee Fee { get { return new TbFee(payment.Fee); } }
        public TbClass Class { get { return new TbClass(payment.Class); } }
        public TbFeeClass FeeClass { get { return new TbFeeClass(payment.FeeGuid); } }
        public int? BillType { get { return payment.BillType; } }
        public string StudentName { get { return payment.Student.StundentName; } }
        public string DivisionName { get { return payment.Student.Division.Division; } }
    }
}
