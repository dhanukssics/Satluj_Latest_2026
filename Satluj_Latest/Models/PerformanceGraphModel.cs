

namespace Satluj_Latest.Models
{
    public class PerformanceGraphModel
    {
       public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long ExamId { get; set; }
        public long SubjectId { get; set; }
        public long UserId { get; set; }
        public long TeacherId { get; set; }
    }
    public class Chart
    {
        public string Heading { get;set;}
        public long SchoolId { get; set; }
        public List<SingleCharts> list { get; set; }
        public string fullList { get; set; }
    }
    public class SingleCharts
    {
        public string ChartName { get; set; }
        public decimal NTP { get; set; }
        public decimal Average { get; set; }
        public decimal Merit { get; set; }
        public decimal Super { get; set; }
        public string SubjectName { get; set; }
        public bool last { get; set; }
    }
}


