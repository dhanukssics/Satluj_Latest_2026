using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class SchoolClassListWithDivisionMapModel
    {
        public long ClassId { get; set; }
        public long SchoolId { get; set; }
        public string ClassName { get; set; }
        public List<AddDivision> Division { get; set; }
    }
}
