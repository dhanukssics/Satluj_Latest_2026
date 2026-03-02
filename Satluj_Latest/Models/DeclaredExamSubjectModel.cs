using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class DeclaredExamSubjectModel
    {
        public long SchoolId { get; set; }
        public long ExamId { get; set; }
        public long SubjectId { get; set; }
        public DateTime ExamTime { get; set; }
        public string ExamTimeString { get; set; }
        public decimal TotalScore { get; set; }
        public string Remark { get; set; }
        public string SubjectName { get; set; }
        public long Id { get; set; }
    }
}