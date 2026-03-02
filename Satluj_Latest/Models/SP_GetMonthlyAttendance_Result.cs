namespace Satluj_Latest.Models
{
    public class SP_GetMonthlyAttendance_Result
    {
        public int Day { get; set; }
        public string Status { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int Morning { get; set; }
    }
}
