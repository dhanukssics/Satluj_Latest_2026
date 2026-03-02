using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Designation:BaseReference
    {
        private TbDesignation designation;
        public Designation(TbDesignation obj) { designation = obj; }
        public Designation(long id) { designation = _Entities.TbDesignations.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return designation.Id; } }
        public long SchoolId { get { return designation.SchoolId; } }
        public string DesignationName { get { return designation.DesignationName; } }
        public string DesignationCode { get { return designation.DesignationCode; } }
        public bool IsActive { get { return designation.IsActive; } }
        public System.DateTime TimeStamp { get { return designation.TimeStamp; } }
    }
}
