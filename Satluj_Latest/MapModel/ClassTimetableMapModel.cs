using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
  public  class ClassTimetableMapModel
    {
        public string Day { get; set; }
        public List<SubjectDetailsList> Details { get; set; }
    }
    public class SubjectDetailsList
    {
        public string Period { get; set; }
        public string Subject { get; set; }
        public string Teacher { get; set; }
    }
}



