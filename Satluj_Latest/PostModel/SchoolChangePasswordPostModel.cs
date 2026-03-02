using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class SchoolChangePasswordPostModel
    {
        public string schoolId { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
