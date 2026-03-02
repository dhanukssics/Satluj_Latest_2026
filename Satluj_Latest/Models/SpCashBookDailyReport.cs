using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    class SpCashBookDailyReport
    {
        public SpCashBookDailyReport()
        {
            
        }
        //DateTime date = DateTime.Now;
        private sp_CashBookDailyReport_Result reoprt;
        public SpCashBookDailyReport(sp_CashBookDailyReport_Result obj) { reoprt = obj; }
        public long DayBookId { get { return reoprt.DayBookId; } }
        public DateTime? EntryDate { get { return reoprt.EntryDate; } }
        public string AccHeadName { get { return reoprt.AccHeadName; } }
        public decimal? Expense { get { return reoprt.Expense; } }
        public decimal? Income { get { return reoprt.Income; } }
        public string VoucherNo { get { return reoprt.VoucherNo; } }
        public string Narration { get { return reoprt.Narration; } }
    }
}
