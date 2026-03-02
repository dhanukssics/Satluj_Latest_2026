using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPCashBookReportData
    {
        public long DayBookId { get; set; }
        public DateTime? EntryDate { get; set; }
        public string AccHeadName { get; set; }
        public decimal? Expense { get; set; }
        public decimal? Income { get; set; }
        public string VoucherNo { get; set; }
        public string Narration { get; set; }
    }
}
