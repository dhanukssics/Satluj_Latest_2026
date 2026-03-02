using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class LibraryStudentBook : BaseReference
    {
        private TbLibraryBookStudent studentBook;
        public LibraryStudentBook(TbLibraryBookStudent obj) { studentBook = obj; }
        public LibraryStudentBook(long id) { studentBook = _Entities.TbLibraryBookStudents.FirstOrDefault(z => z.BookId == id); }
        public long studentBookId { get { return studentBook.StudentBookId; } }
        public long bookId { get { return studentBook.BookId; } }
        public bool status { get { return studentBook.Status; } }
        public bool isActive { get { return studentBook.IsActive; } }
        public DateTime issueDateTime { get { return studentBook.IssueDateTime; } }
        public DateTime? acceptDateTime { get { return studentBook.AcceptDateTime; } }

        public Student Student { get { return new Student(studentBook.StudentId); } }

    }
}
