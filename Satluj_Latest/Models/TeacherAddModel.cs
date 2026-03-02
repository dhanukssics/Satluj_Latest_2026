namespace Satluj_Latest.Models
{
    public class TeacherAddModel
    {
        public long schoolId { get; set; }
        public string teacherName { get; set; }
        public string emailId { get; set; }
        public string contactNumber { get; set; }
        public long DesignationId { get; set; }
        public long DepartmentId { get; set; }
        public string RolesData { get; set; }
        public long? UserTypeId { get; set; }
        public long classId { get; set; }
        public long divisionId { get; set; }
    }
}
