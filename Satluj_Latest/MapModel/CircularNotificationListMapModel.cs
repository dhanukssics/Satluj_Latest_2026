using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
   public class CircularNotificationListMapModel
    {
        public CircularNotificationListMapModel()
        {
            
        }
        public string Head { get; set; }
        public long CircularId { get; set; }
        public long SchoolId { get; set; }
        public DateTime CircularDate { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public DateTime TimeStamp { get; set; }
        public int FromStatus { get; set; }//0: Circular,1:Event
        public string EventHead { get; internal set; }
    }
}
