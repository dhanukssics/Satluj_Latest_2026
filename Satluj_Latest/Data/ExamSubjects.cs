using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ExamSubjects : BaseReference
    {
        private readonly TbExamSubject _examSubject;

       
        public ExamSubjects(TbExamSubject entity)
        {
            _examSubject = entity ?? throw new ArgumentNullException(nameof(entity));
        }

        
        public ExamSubjects(long id)
        {
            _examSubject = _Entities.TbExamSubjects
                .Include(x => x.SubjectNavigation)
                .FirstOrDefault(x => x.SubId == id)
                ?? throw new Exception($"Subject with Id {id} not found.");
        }
        public long SubId => _examSubject.SubId;
        public long ExamId => _examSubject.ExamId;
        public string Subject => _examSubject.Subject;
        public decimal Mark => _examSubject.Mark;
        public bool IsActive => _examSubject.IsActive;
        public DateTime TimeStamp => _examSubject.TimeStamp;
        public decimal InternalMarks => _examSubject.InternalMarks;
        public decimal ExternalMark => _examSubject.ExternalMark;
        public DateTime ExamDate => _examSubject.ExamDate;
        public long SubjectId => _examSubject.SubjectId;

        public string SubjectName => _examSubject.SubjectNavigation?.SubjectName ?? "N/A";
    }
}
