using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
  public  class SchoolTravelHistoryPostModel
    {
      public string busId { get; set; }
      public string tripNumber { get; set; }
      public string dateTime { get; set; }
      public string timeStatus { get; set; } // 0==Morning ,1==Evening
    }
}
