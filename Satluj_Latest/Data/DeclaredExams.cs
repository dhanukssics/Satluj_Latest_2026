using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class DeclaredExams : BaseReference
    {
        private TbDeclaredExam exams;
        public DeclaredExams(TbDeclaredExam obj) { exams = obj; }
        public DeclaredExams(long Id) { exams = _Entities.TbDeclaredExams.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return exams.Id; } }
        public long SchoolId { get { return exams.SchoolId; } }
        public long ExamId { get { return exams.ExamId; } }
        public long ClassId { get { return exams.ClassId; } }
        public System.DateTime StartDate { get { return exams.StartDate; } }
        public System.DateTime EndDate { get { return exams.EndDate; } }
        public bool IsActive { get { return exams.IsActive; } }
        public System.DateTime TimeStamp { get { return exams.TimeStamp; } }
        public long TermId { get { return exams.TermId ?? 0; } }
        public string TermName { get { return exams.TbExamTerm.DefaultExam; } }
        public string ExamName { get { return exams.Exam.ExamName; } }
        public string ClassName { get { return exams.Class.Class; } }
    }
}
