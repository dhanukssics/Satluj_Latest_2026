using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
  public  class ParentAddKidOTPPostModel
    {
        public string parentId { get; set; }
        public string kidSpecialId { get; set; }
        public string OTP { get; set; }
        public string SchoolId { get; set; }
    }
}
