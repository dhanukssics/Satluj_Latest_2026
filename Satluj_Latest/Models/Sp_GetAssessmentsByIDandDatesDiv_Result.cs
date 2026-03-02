namespace Satluj_Latest.Models
{
    public partial class Sp_GetAssessmentsByIDandDatesDiv_Result
    {
        public int ClassId { get; set; }
        public Nullable<int> Division { get; set; }
        public Nullable<System.DateTime> AttendanceDate { get; set; }
        public string Filename { get; set; }
        public string TeacherName { get; set; }
        public string SubjectName { get; set; }
        public string Description { get; set; }
    }
}
