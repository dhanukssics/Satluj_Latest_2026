using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class TeacherMessagePostModel
    {
        public string teacherId { get; set; }
        public string divisionId { get; set; }
        public string index { get; set; }
        public string length { get; set; }
        public string studentId { get; set; }//0: If needs all messages Otherwise particular student Messages
    }
}
