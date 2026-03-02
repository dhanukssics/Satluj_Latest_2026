namespace Satluj_Latest.Models
{
    public class TeacherSubjectRelationModel
    {
        public long SchoolId { get; set; }
        public long TeacherId { get; set; }
        public List<SchoolValue.TeacherListModel> TeacherListModel { get; set; }
    }
}
