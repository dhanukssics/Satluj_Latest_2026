using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class LedgerReportModel
    {
        public long SchoolId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long HeadId { get; set; }
        public long SubId { get; set; }
        public string SchoolName { get; set; }
        public string Heading { get; set; }
    }
}