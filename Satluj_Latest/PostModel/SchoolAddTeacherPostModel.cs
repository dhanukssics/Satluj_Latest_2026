using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class SchoolAddTeacherPostModel 
    {
        public string schoolId { get; set; }
        public string teacherName { get; set; }
        public string classId { get; set; }
        public string divisionId { get; set; }
        public string contactNumber { get; set; }
        public string emailId { get; set; }
        public string image { get; set; }
        public string filePath { get; set; }
        public string DesignationId { get; set; }
        public string DepartmentId { get; set; }
        public string RolesData { get; set; }
        public string UserTypeId { get; set; }
    }
}
