using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class AcademicPeriods:BaseReference
    {
        private TbAcademicPeriod ap;
        public AcademicPeriods(TbAcademicPeriod obj) { ap = obj; }

        public AcademicPeriods(long Id) { ap = _Entities.TbAcademicPeriods.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return ap.Id; } }
        public long ClassId { get { return ap.ClassId; } }
        public string ClassName { get { return ap.Class.Class; } }
        public long SchoolId { get { return ap.SchoolId; } }
        public System.DateTime StartDate { get { return ap.StartDate; } }
        public System.DateTime EndDate { get { return ap.EndDate; } }
        public string PeriodsName { get { return ap.PeriodsName; } }
        public bool IsActive { get { return ap.IsActive; } }
        public System.DateTime TimeStamp { get { return ap.TimeStamp; } }
        public long RegionId { get { return ap.RegionId; } }
    }
}
