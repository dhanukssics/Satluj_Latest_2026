using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class LaboratoryCategory : BaseReference
    {
        private TbLaboratoryCategory category;
        public LaboratoryCategory(TbLaboratoryCategory obj) { category = obj; }
        public LaboratoryCategory(long id) { category = _Entities.TbLaboratoryCategories.FirstOrDefault(z => z.CategoryId == id); }
        public long categoryId { get { return category.CategoryId; } }
        public string laboratoryName { get { return category.LaboratoryName; } }
        public long schoolId { get { return category.SchoolId; } }
        public bool IsActive { get { return category.IsActive; } }
    }
}
