namespace Satluj_Latest.Models
{
    public class SPdeleteoldAttendence
    {
        public SPdeleteoldAttendence()
        {
            
        }
        public DateTime attendancedate { get;set;}
        public int classId { get; set; }
        public long divisionId { get; set; }
        public int shiftStatus { get; set; }

    }
}
