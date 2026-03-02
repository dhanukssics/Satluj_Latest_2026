using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class StudentMarks : BaseReference
    {
        private TbStudentMark studentMarks;
        public StudentMarks(TbStudentMark obj) { studentMarks = obj; }
        public StudentMarks(long id) { studentMarks = _Entities.TbStudentMarks.FirstOrDefault(z => z.MarkId == id); }

        

        public long MarkId { get { return studentMarks.MarkId; } }
        public long StudentId { get { return studentMarks.StudentId; } }
        public long ExamId { get { return studentMarks.ExamId; } }
        public long SubjectId { get { return studentMarks.SubjectId; } }
        public long Mark { get { return studentMarks.Mark; } }
        public bool IsActive { get { return studentMarks.IsActive; } }
        public System.DateTime TimeStamp { get { return studentMarks.TimeStamp; } }
        public decimal? InternalMark { get { return studentMarks.InternalMark; } }
        public decimal? ExternalMark { get { return studentMarks.ExternalMark; } }

        public TbStudentMark X { get; }
    }
}
