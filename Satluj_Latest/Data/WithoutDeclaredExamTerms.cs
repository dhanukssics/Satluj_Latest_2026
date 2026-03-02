using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class WithoutDeclaredExamTerms:BaseReference
    {
        private sp_WithoutDeclaredExamTerms_Result otherTerms;
        public WithoutDeclaredExamTerms(sp_WithoutDeclaredExamTerms_Result obj) { otherTerms = obj; }
        public long Id { get { return otherTerms.Id; } }
        public string DefaultExam { get { return otherTerms.DefaultExam; } }
        public bool IsActive { get { return otherTerms.IsActive; } }
    }
}
