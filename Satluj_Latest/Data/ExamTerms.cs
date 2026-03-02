using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ExamTerms : BaseReference
    {
        private TbExamTerm terms;
        public ExamTerms(TbExamTerm obj) { terms = obj; }
        public ExamTerms(long Id) { terms = _Entities.TbExamTerms.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return terms.Id; } }
        public string DefaultExam { get { return terms.DefaultExam; } }
        public bool IsActive { get { return terms.IsActive; } }
    }
}
