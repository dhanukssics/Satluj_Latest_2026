using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class CommonModel
    {

    }
    public class RegionDataWithClass
    {
        public int Slno { get; set; }
        public string RegionName { get; set; }
        public string ClaassList { get; set; }
        public long RegionId { get; set; }
    }
    public class EventsList
    {
        public long EventId { get; set; }
        public string EventHead { get; set; }
        public DateTime EventDate { get; set; }
        public string Description { get; set; }
    }
    public class CircularsList
    {
        public long CircularId { get; set; }
        public string FilePath { get; set; }
        public DateTime CircularDate { get; set; }
        public string Description { get; set; }
        public string CircularHead { get; set; }
    }
    public class UserModule
    {
        public long MainId { get; set; }
        public long SubId { get; set; }
    }
    public class AttendanceSummary
    {
        public int Term { get; set; }
        public int Total { get; set; }
        public int Present { get; set; }
    }
    public class HealthSummay
    {
        public int Term { get; set; }
        public decimal Weight { get; set; }
        public decimal Height { get; set; }
    }
    public class RemarksData
    {
        public int Term { get; set; }
        public string Remark { get; set; }
    }
    public class StudentOutOfScores
    {
        public decimal MarkObtained { get; set; }
        public decimal Percentage { get; set; }
    }
    public class V_AggregateScore
    {
        public int AssId { get; set; }
        public decimal AggregateScore { get; set; }
        public string Grade { get; set; }
    }
}
