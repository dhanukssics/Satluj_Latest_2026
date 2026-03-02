using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
   public class PasswordCheckingPostModel
    {
        public long SchoolId { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
