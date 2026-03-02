using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPDayBookStatus
    {
        public SPDayBookStatus(SPDayBookStatus result)
        {
            Result = result;
        }
        public SPDayBookStatus()
        {

        }
        public decimal? OpeningBalance { get; set; }
        public decimal? ClosingBalance { get; set; }
        public decimal? TotalDebit { get; set; }
        public decimal? TotalCredit { get; set; }
        public SPDayBookStatus Result { get; }
    }
}
