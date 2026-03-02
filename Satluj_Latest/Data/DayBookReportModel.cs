using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class DayBookReportModel
    {
        public DateTime EnterDate { get; set; }
        public decimal Opening { get; set; }
        public decimal Closing { get; set; }
        public List<DayBookReportDetails> _list { get; set; }
    }
    public class DayBookReportDetails
    {
        public string VoucherNo { get; set; }
        public string AccountHeadName { get; set; }
        public string SubLedger { get; set; }
        public decimal IncomeAmount { get; set; }
        public decimal ExpenseAmount { get; set; }
        public string TransactionType { get; set; }
        public string Narration { get; set; }
        public string FromStatus { get; set; }
    }
}
