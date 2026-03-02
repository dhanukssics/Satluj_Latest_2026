using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public  class TeacherAttendancePostModel
    {
        public class Student
        {
            public string studentId { get; set; }
            public string attendaneStatus { get; set; } //Check In=1 Check Out =0;
           
        }
        public class TeacherAttendanceDataListPostModel
        {
            public string teacherId { get; set; }
            public string classId { get; set; }
            public string divisionId { get; set; }
            public string attendanceDateTime { get; set; }
            public string shiftstatus { get; set; }// Morning =0 Afternoon=1 This is changed in 07-Jun-2018 
            public List<Student> studentList { get; set; }
            //public Student studentOne { get; set; }
        }
    }
}
