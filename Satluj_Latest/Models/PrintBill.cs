using Satluj_Latest.Utility;

namespace Satluj_Latest.Models
{
    public class PrintBill
    {
        public long StudentId { get; set; }
        public List<TbPayment> PaymentList { get; set; }
        public TbStudentPaidAmount StudentPaidFee { get; set; }
        public long BillNumber { get; set; }
    }

}
