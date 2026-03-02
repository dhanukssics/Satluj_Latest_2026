using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class TeacherClassData
    {
        public long TeacherClassId { get; set; }
        public long TeacherId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public string ClassName { get; set; }
        public string DivisionName { get; set; }
    }
}
