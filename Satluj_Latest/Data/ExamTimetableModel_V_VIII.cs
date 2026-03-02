using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ExamTimetableModel_V_VIII
    {
        public string SchoolName { get; set; }
        public string SchoolAdddress { get; set; }
        public string SchoolLogo { get; set; }
        public string ExamName { get; set; }
        public string ClassName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<TimeTable_V_VIII> List { get; set; }
    }

    public class TimeTable_V_VIII
    {
        public DateTime ExamDate { get; set; }
        public string Subject1 { get; set; }
        public string Subject2 { get; set; }
    }
}
