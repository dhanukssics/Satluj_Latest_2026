

namespace Satluj_Latest.Models
{
    public class MonthlyAttandanceModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public AttendanceShift ShiftId { get; set; }
        public DateTime AttandanceDate { get; set; }
        public int shiftData { get; set; }
    }
}