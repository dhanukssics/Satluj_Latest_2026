using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class BankEntryReportModel
    {
        public System.DateTime EnterDate { get; set; }
        public string VoucherNumber { get; set; }
        public string TransactionType { get; set; }
        public string BankName { get; set; }
        public string AccHeadName { get; set; }
        public Nullable<decimal> Deposit { get; set; }
        public Nullable<decimal> Withdraw { get; set; }
        public string Narration { get; set; }
    }
}
