using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ExamTimetableModel
    {
        public string SchoolName { get; set; }
        public string SchoolAdddress { get; set; }
        public string SchoolLogo { get; set; }
        public string ExamName { get; set; }
        public string ClassName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<TimeTable> List { get; set; }
    }
    public class TimeTable
    {
        public DateTime ExamDate { get; set; }
        public string Subject1 { get; set; }
        public string Subject2 { get; set; }
    }
}