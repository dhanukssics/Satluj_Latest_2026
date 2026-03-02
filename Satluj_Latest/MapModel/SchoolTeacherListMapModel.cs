using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class SchoolTeacherListMapModel
    {
        public long TeacherId { get; set; }
        public string TeacherSpecialId { get; set; }
        public string TeacherName { get; set; }
        public long SchoolId { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
         public System.DateTime TimeStamp { get;set;}
         public System.Guid TeacherGuid { get;set;}
         public bool IsActive { get;set;}
         public string FilePath { get;set;}
         public List<TeacherClassData> TeacherClass { get; set; }
        //public TeacherClassData TeacherClass { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string RolesString { get; set; }

    }
}
