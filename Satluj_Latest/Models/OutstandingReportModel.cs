using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class OutstandingReportModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long FeeId { get; set; }
        public List<ReportDateList> ReportList { get; set; }
    }
    public class ReportDateList
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public long ClassId { get; set; }
        public string ClassName { get; set; }
        public long DivisionId { get; set; }
        public string DivisionName { get; set; }
        public decimal Amount { get; set; }
        public string ContactNumber { get; set; }
        public int ClassOrder { get; set; }
    }
    public class OutstandingReportNew
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