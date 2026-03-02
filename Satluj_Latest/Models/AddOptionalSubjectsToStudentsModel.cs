using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class AddOptionalSubjectsToStudentsModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long SubjectId { get; set; }
        public List<StudentListData> Data { get; set; }
    }
    public class StudentListData
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public string FilePath { get; set; }
        public string SpecialId { get; set; }
        public string ParentName { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public long SubjectId { get; set; }
    }
}