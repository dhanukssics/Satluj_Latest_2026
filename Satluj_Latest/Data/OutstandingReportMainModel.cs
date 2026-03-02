using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class OutstandingReportMainModel
    {
        public long SchoolId { get; set; }
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public string ClassDetails { get; set; }
        public string ContactNumber { get; set; }
        public decimal Total { get; set; }
        public int ClassOrder { get; set; }
        public long DivisionId { get; set; }
        public List<SubList> SubList { get; set; }
    }
    public class SubList
    {
        public long FeeId { get; set; }
        public string FeeName { get; set; }
        public decimal Amount { get; set; }
    }
}
