using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class StudentsCoScholasticAreaResultModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long ExamId { get; set; }
        public List<StudentCoScholasticMarkList> ListData { get; set; }
        public bool IsEntered { get; set; }
        public long CoScholasticId { get; set; }
    }
    public class StudentCoScholasticMarkList
    {
        public string StudentName { get; set; }
        public string Mark { get; set; }
        public long StudentId { get; set; }
        public int RollNo { get; set; }
    }
}