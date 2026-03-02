using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ExamResult : BaseReference
    {
        private TbExamResult result;
        public ExamResult(TbExamResult obj) { result = obj; }
        public ExamResult(long Id) { result = _Entities.TbExamResults.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return result.Id; } }
        public long SchoolId { get { return result.SchoolId; } }
        public long StudentId { get { return result.StudentId; } }
        public long ExamId { get { return result.ExamId; } }
        public long SubjectId { get { return result.SubjectId; } }
        public decimal StudentScore { get { return result.StudentScore; } }
        public bool IsActive { get { return result.IsActive; } }
        public System.DateTime TimeStamp { get { return result.TimeStamp; } }
    }
}
