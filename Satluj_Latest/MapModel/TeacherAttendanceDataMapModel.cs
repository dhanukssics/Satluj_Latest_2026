using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class TeacherAttendanceDataMapModel
    {
        public long AttendanceId { get; set; }
        public bool AttendanceData { get; set; }
        public long StudentId { get; set; }
        public int ShiftStatus { get; set; }
    }
}
