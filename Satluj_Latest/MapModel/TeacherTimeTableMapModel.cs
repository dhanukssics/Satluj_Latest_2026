using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class TeacherTimeTableMapModel
    {
        public string Day { get; set; }
        public List<DetailsList> Details { get; set; }
    }
    public class DetailsList
    {
        public string Period { get; set; }
        public string Subject { get; set; }
        public string Classdetails { get; set; }
    }
}
