
namespace Satluj_Latest.Models
{
    public class TimetableModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisonId { get; set; }
        public long TeacherId { get; set; }
        public long SubjectId { get; set; }
        public Days DayId { get; set; }
        public Periods Period { get; set; }
        public long TableId { get; set; }
        public string SchoolName { get; set; }
        public string ClassName { get; set; }
        public string DivisionName { get; set; }
    }
}