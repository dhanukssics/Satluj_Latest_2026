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

        public DeclaredExamSubjects(TbDeclaredExamSubject obj)
        {
            sub = obj;
        }

        public DeclaredExamSubjects(long Id)
        {
            sub = _Entities.TbDeclaredExamSubjects.FirstOrDefault(z => z.Id == Id);
        }

        public long Id => sub?.Id ?? 0;

        public long DeclaredExamId => sub?.DeclaredExamId ?? 0;

        public long SubjectId => sub?.SubjectId ?? 0;

        public string Subject
        {
            get
            {
                if (sub?.Subject != null)
                    return sub.Subject.SubjectName;

                var subject = _Entities.TbSubjects
                    .FirstOrDefault(x => x.SubId == sub.SubjectId && x.IsActive);

                return subject?.SubjectName ?? "";
            }
        }

        public DateTime ExamDate => sub?.ExamDate ?? DateTime.MinValue;

        public decimal TotalScore => sub?.TotalScore ?? 0;

        public string Remark => sub?.Remark ?? "";

        public bool IsActive => sub?.IsActive ?? false;

        public DateTime TimeStamp => sub?.TimeStamp ?? DateTime.MinValue;
    }
}

