using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class Common
    {

    }
    //public class data
    //{
    //    public int shiftStatusMorning { get; set; }
    //    public int shiftStatusEvening { get; set; }
    //    public int attendanceStatusMorning { get; set; }
    //    public int attendanceStatusEvening { get; set; }
    //}
    public class AttendanceDetails // Old 
    {
        public string attendanceDate { get; set; }
        public bool mornignShift { get; set; }
        public bool eveningShift { get; set; }
    }
    public class NewAttendanceDetails // Now using 
    {
        public string attendanceDate { get; set; }
        public int mornignShift { get; set; }
        public int eveningShift { get; set; }
    }
}
