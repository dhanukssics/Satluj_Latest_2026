using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class PerformanceModel
    {
        public long Id { get; set; }
        public long SchoolId { get; set; }
        public long TeacherId { get; set; }
        public long SubjectId { get; set; }
        public List<ClassMainList> ClassList { get; set; }
    }
    public class ClassMainList
    {
        public long ClassId { get; set; }
        public string ClassName { get; set; }
        public bool IsFullExists { get; set; }
        public List<ClassDivisiomList> DivisionList { get; set; }
        public string DivisionListIdString { get; set; }
    }
    public class ClassDivisiomList
    {
        public long DivId { get; set; }
        public string DivisionName { get; set; }
        public bool IsExits { get; set; }
    }
}