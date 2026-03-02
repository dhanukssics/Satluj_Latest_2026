using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ReceiptPaymentDataModel
    {

        public string Receipt { get; set; }
        public decimal ReceiptAmount { get; set; }
        public string Payment { get; set; }
        public decimal  PaymentAmount { get; set; }
        public int Type { get; set; }
        public string FromType { get; set; }
        public string ReceiptNarration { get; set; }
        public string PaymentNarration { get; set; }
    }
}
