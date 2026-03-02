using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class BookCategory : BaseReference
    {
        private TbBookCategory category;
        public BookCategory(TbBookCategory obj) { category = obj; }
        public BookCategory(long id) { category = _Entities.TbBookCategories.FirstOrDefault(z => z.CategoryId == id); }
        public long categoryId { get { return category.CategoryId; } }
        public string categoryName { get { return category.Category; } }
        public long schoolId { get { return category.SchoolId; } }
        public bool IsActive { get { return category.IsActive; } }
    }
}
