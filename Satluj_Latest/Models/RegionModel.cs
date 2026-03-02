using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class RegionModel
    {
        public long SchoolId { get; set; }
        public string RegionName { get; set; }
        public List<ClassList> _ClassList { get; set; }
        public long Id { get; set; }
    }
    public class ClassList
    {
        public long ClassId { get; set; }
        public bool IsRegion { get; set; }
        public string ClassName { get; set; }
    }

}