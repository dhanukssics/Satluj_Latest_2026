using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Department:BaseReference
    {
        private TbDepartment department;
        public Department(TbDepartment obj) { department = obj; }
        public Department(long id) { department = _Entities.TbDepartments.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return department.Id; } }
        public long SchoolId { get { return department.SchoolId; } }
        public string DepartmentName { get { return department.DepartmentName; } }
        public string Abbreviation { get { return department.Abbreviation; } }
        public bool IsActive { get { return department.IsActive; } }
        public System.DateTime TimeStamp { get { return department.TimeStamp; } }
    }
}
