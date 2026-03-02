using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class StudentsMarkListData
    {
        public long ExamId { get; set; }
        public long SubjectId { get; set; }
        public List<StudentList> _ListData { get; set; }
    }
}