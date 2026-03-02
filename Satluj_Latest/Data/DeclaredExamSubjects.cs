using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class DeclaredExamSubjects : BaseReference
    {
        private TbDeclaredExamSubject sub;
        public DeclaredExamSubjects(TbDeclaredExamSubject obj) { sub = obj; }
        public DeclaredExamSubjects(long Id) { sub = _Entities.TbDeclaredExamSubjects.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return sub.Id; } }
        public long DeclaredExamId { get { return sub.DeclaredExamId; } }
        public long SubjectId { get { return sub.SubjectId; } }
        public string Subject { get { return sub.Subject.SubjectName; } }
        public System.DateTime ExamDate { get { return sub.ExamDate; } }
        public decimal TotalScore { get { return sub.TotalScore; } }
        public string Remark { get { return sub.Remark; } }
        public bool IsActive { get { return sub.IsActive; } }
        public System.DateTime TimeStamp { get { return sub.TimeStamp; } }
    }
}
