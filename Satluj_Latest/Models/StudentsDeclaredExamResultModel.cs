using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class StudentsDeclaredExamResultModel
    {
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long ExamId { get; set; }
        public long SubjectId { get; set; }
        public List<StudentMarkList> ListData { get; set; }
        public bool IsEntered { get; set; }
        public decimal TotalScore { get; set; }
        public bool IsComputerScience { get; set; }// The computer science subjeect have theory and practical, its not have the Scolastic and Co Scolastic Results 
        public bool IsDance { get; set; }//dance(kathak) have theory and practical
        public bool IsLegalStudies { get; set; }//legalstudies have theory and practical
    }
    public class StudentMarkList
    {
        public string StudentName { get; set; }
        public string Mark { get; set; }
        public string PracticalScore { get; set; }//Only for Computer Science 
        public long StudentId { get; set; }
        public int ROllNo { get; set; }
    }
}