using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class StudentMarksEntry
    {
        public long SchoolId { get; set;}
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long ExamId { get; set; }
        public long SubjectId { get; set; }
        public long UserId { get; set; }
    }
}