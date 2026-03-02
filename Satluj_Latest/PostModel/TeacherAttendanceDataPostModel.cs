using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
   public class TeacherAttendanceDataPostModel
    {
       public string teacherId { get; set; }
       public string classId { get;set; }
       public string divisionId { get; set; }
       public string date { get; set; }
       public string shiftStatus { get; set; }
    }
}
