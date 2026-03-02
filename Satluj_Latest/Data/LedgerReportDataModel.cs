using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class LedgerReportDataModel
    {
        public string AccountHead { get; set; }
        public List<LedgerDetailsList> _LedgerDetailsList { get; set; }
    }
    public class LedgerDetailsList
    {
        public DateTime AccountDate { get; set; }
        public string SubLedger { get; set; }
        public decimal Income { get; set; }
        public decimal Expence { get; set; }
        public string Status { get; set; }
    }
}
