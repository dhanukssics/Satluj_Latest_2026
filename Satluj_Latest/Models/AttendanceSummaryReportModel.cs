using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class AttendanceSummaryReportModel
    {
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolLogo { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string ClassName { get; set; }
        public string DivisionName { get; set; }
        public int TotalStudents { get; set; }
        public int Present { get; set; }
        public int Absent { get; set; }
        public string InCharge { get; set; }
    }
}