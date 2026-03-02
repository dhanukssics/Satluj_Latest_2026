using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class ParentChangePasswordPostModel
    {
        public string ParentId { get; set; }
        public string OldPassword { get; set; }
        public string CurrentPassword { get; set; }
    }
}
