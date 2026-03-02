using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class LibraryBook : BaseReference
    {
        private TbLibraryBook book;
        public LibraryBook(TbLibraryBook obj) { book = obj; }
        public LibraryBook(long id) { book = _Entities.TbLibraryBooks.FirstOrDefault(z => z.BookId == id); }
        public long bookId { get { return book.BookId; } }
        public long serialNo { get { return book.SerialNumber; } }
        public long categoryId { get { return book.CategoryId; } }
        public string title { get { return book.Title; } }
        public string author { get { return book.Author; } }
        public int status { get { return book.Status; } }
        public bool IsActive { get { return book.IsActive; } }
        public BookCategory Category { get { return new BookCategory(book.Category); } }

        public string ReferenceNumber { get { return book.ReferenceNumber; } }
        public List<LibraryStudentBook> GetBookStudentList()
        {
            return book.TbLibraryBookStudents.Where(z => z.IsActive).ToList().Select(x => new LibraryStudentBook(x)).ToList();
        }
    }
}
