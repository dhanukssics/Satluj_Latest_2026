using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ReceiptPaymentNewModel
    {
        public string AccountHeadReceipt { get; set; }
        public string AccountHeadPayment { get; set; }
        public int FromData { get; set; }
        public long Id { get; set; }
        public decimal receipt { get; set; }
        public decimal payment { get; set; }
        public int SLNo { get; set;}
    }
}
