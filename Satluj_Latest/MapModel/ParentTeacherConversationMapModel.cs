using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
   public class ParentTeacherConversationMapModel
    {
        public string StudentId { get; set; }
        public int RequirestType { get; set; } // 0: Parent , 1: Teacher
        public int length { get; set;  }
        public int start { get; set; }
    }
    public class ParentTeacherMapModel
    {
        public string StudentId { get; set; }
        public int length { get; set; }
    }
    public class TeacherConversationMapModel
    {
        public string StudentId { get; set; }
        public string TeacherId { get; set; }
        public int Length { get; set; }
    }
}
