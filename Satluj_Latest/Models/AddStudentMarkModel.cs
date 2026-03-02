using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class AddStudentMarkModel
    {
      
        public long SubjectId { get; set; }
        public double StudentMark { get; set; }
        public long SchoolId { get; set; }
        public long DivisionId { get; set; }
        public long ClassId { get; set; }
        public long ExamId { get; set; }
        public long TotalMark { get; set; }
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Enter valid Mark")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Enter valid Mark")]
        public decimal TotalInternalMark { get; set; }
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Enter valid Mark")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Enter valid Mark")]
        public decimal TotalExternalMark { get; set; }
        public List<StudentList> StudentList { get; set; }
    }
    public class StudentList
    {
        public string StudentName { get; set; }
        public long StudentId { get; set; }
        public decimal InternalMark { get; set; }
        public decimal ExternalMark { get; set; }
        public long Total { get; set; }
        public long ExamId { get; set; }
        public long SubjectId { get; set; }
    }
}