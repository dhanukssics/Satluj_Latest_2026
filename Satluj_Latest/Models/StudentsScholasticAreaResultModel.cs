using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class StudentsScholasticAreaResultModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long ExamId { get; set; }
        public long SubjectId { get; set; }
        public List<StudentScholasticMarkList> ListData { get; set; }
        public bool IsEntered { get; set; }
        public decimal TotalScore { get; set; }
        public long ScholasticId { get; set; }
    }
    public class StudentScholasticMarkList
    {
        public string StudentName { get; set; }
        public string Mark { get; set; }
        public long StudentId { get; set; }
        public int RollNo { get; set; }
    }
}