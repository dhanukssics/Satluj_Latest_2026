using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class TeacherResetPasswordModel
    {
        public string TeacherId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

    }
}
