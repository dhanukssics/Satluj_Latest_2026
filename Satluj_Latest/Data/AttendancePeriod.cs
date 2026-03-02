using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class AttendancePeriod : BaseReference
    {
        private TbAttendancePeriod ap;
        public AttendancePeriod(TbAttendancePeriod obj) { ap = obj; }
        public AttendancePeriod(long id) { ap = _Entities.TbAttendancePeriods.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return ap.Id; } }
        public long SchoolId { get { return ap.SchoolId; } }
        public long StudentId { get { return ap.StudentId; } }
        public long DivisionId { get { return ap.DivisionId; } }
        public long PeriodId { get { return ap.PeriodId; } }
        public long UserId { get { return ap.UserId; } }
        public int TotalDays { get { return ap.TotalDays; } }
        public int PresentDays { get { return ap.PresentDays; } }
        public bool IsActive { get { return ap.IsActive; } }
        public System.DateTime TimeStampp { get { return ap.TimeStampp; } }
    }
}
