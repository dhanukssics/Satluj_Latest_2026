using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Exams : BaseReference
    {
        private TbExam exams;
        public Exams(TbExam obj) { exams = obj; }
        public Exams(long id) { exams = _Entities.TbExams.FirstOrDefault(z => z.ExamId == id); }
        public long ExamId { get { return exams.ExamId; } }
        public long SchoolId { get { return exams.SchoolId; } }
        public long UserId { get { return exams.UserId; } }
        public long ClassId { get { return exams.ClassId; } }
        //public long DivisionId { get { return exams.DivisionId; } }
        public string ExamName { get { return exams.ExamName; } }
        //public System.DateTime ExamDate { get { return exams.ExamDate; } }
        public bool IsActive { get { return exams.IsActive; } }
        public System.DateTime TimeStamp { get { return exams.TimeStamp; } }
        public Nullable<System.DateTime> StartDate { get { return exams.StartDate; } }
        public Nullable<System.DateTime> Enddate { get { return exams.EndDate; } }
        public string ClassName { get { return exams.Class.Class; } }
        //public string DivisionName { get { return exams.tb_Division.Division; } }
        public List<ExamSubjects> ExamSubjectsList { get { return exams.TbExamSubjects.Where(x => x.ExamId == exams.ExamId && x.IsActive).ToList().Select(x => new ExamSubjects(x)).ToList(); } }
        public  Exams(long SubjectId, long SchoolId)
        {
            var data = _Entities.TbExamSubjects.Where(x => x.SubId == SubjectId && x.IsActive).FirstOrDefault();
             exams = _Entities.TbExams.Where(x => x.SchoolId == SchoolId && x.ExamId == data.ExamId && x.IsActive).FirstOrDefault();
        }
       
    }
}
