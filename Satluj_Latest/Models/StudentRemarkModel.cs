using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class StudentRemarkModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long ExamId { get; set; }
        public List<StudentRemark> list { get; set; }
    }
    public class StudentRemark
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public string Remark { get; set; }
        public int RollNo { get; set; }
        public string AdditionalRemark { get; set; }
    }
}