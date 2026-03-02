using Satluj_Latest.Data;

namespace Satluj_Latest.Models.ViewModels
{
    public class TeachervViewModel
    {
        public long SchoolId { get; set; }
        public List<TeacherSubjectRelationModel> teacherSubjectRelations { get; set; }
        public bool IsAdmin { get; set; }
        public List<Teacher> Teachers { get; set; }
    }
}
