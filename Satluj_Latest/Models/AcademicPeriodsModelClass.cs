using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class AcademicPeriodsModelClass
    {
        public long SchoolId { get; set; }
        public long RegionId { get; set; }
        public long ClassId { get; set; }
        public string StartDatestring { get; set; }
        public string EndDatestring { get; set; }
        public string PeriodsName { get; set; }
        public string ClassName { get; set; }
        public List<AcademicPeriods> PeriodList { get; set; }
    }
}