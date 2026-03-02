using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class StudentListForProgressCardModel
    {
        public long ExamId { get; set; }
        public long SchoolId { get; set; }
        public string Class { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public string Division { get; set; }
        public List<StudentListWithExam> _list { get; set; }
    }
    public class StudentListWithExam
    {
        public long StudnetId { get; set; }
        public string StudnetName { get; set; }
        public int RollNo { get; set; }
    }
}