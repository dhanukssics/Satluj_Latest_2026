using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class HealthResultModel
    {
        public long ClassId { get; set; }
        public long SchoolId { get; set; }
        public long DivisionId { get; set; }
        public long PeriodId { get; set; }
        public List<StudentListForHealth> _StudentListForHealth { get; set; }
    }
    public class StudentListForHealth
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public int RollNo { get; set; }
        public int WorkingDays { get; set; }
        public int PresentDays { get; set; }
    }
}