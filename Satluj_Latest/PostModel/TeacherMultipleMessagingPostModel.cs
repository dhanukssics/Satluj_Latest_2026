using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class TeacherMultipleMessagingPostModel
    {
        public class StudentList
        {
            public string studentId { get; set; }
        }
        public class MultipleMessage
        {
            public string teacherId { get; set; }
            public string subject { get; set; }
            public string description { get; set; }
            public List<StudentList> studentList { get; set; }
        }

    }
}
