using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Satluj_Latest.Data;

namespace Satluj_Latest.Models
{
    public class SummaryModel
    {
        public long SchoolId { get; set; }
        public DateTime DatetimeString { get; set; }
        public List<ClassListForSummary> _list { get; set; }
    }
    public class ClassListForSummary
    {
        public long ClassId { get; set; }
        public string ClassName { get; set; }
    }
    public class AttendanceSummaryFull
    {
       public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolLogo { get; set; }
        public DateTime AttendanceDate { get; set; }
        public List<AttendanceSummaryReportModel> _list = new List<AttendanceSummaryReportModel>();
    }
    public class TimetableSummaryFull
    {
        public string SchoolName { get; set; }
        public string SchoolAddress { get; set; }
        public string SchoolLogo { get; set; }
        public List<ListOfTimetable> _list = new List<ListOfTimetable>();
    }
    public class ListOfTimetable
    {
        public string ClassName { get; set; }
        public string DivisionName { get; set; }
        public List<TimetableListingModel> list = new List<TimetableListingModel>();
    }
    public class AttSummaryModel
    {
        public string IdList { get; set; }
        public string DateString { get; set; }
    }
}