using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ExamBook:BaseReference
    {
        private TbExamBook book;
        public ExamBook(TbExamBook obj) { book = obj; }
        public ExamBook(long Id) { book = _Entities.TbExamBooks.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return book.Id; } }
        public long TermId { get { return book.TermId; } }
        public string ExamName { get { return book.ExamName; } }
        public long SchoolId { get { return book.SchoolId; } }
        public bool IsActive { get { return book.IsActive; } }
        public System.DateTime TimeStamp { get { return book.TimeStamp; } }

    }
}
