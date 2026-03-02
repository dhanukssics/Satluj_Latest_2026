using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
   public class SchoolTravelHistoryMapModel
    {
        public long TripId { get;set;}
        public long DriverId { get; set; }
        public long SchoolId { get; set; }
        public string TripNumber { get; set; }
        public string LocationStart { get; set; }
        public string LocationEnd { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime? ReachTime { get; set; }
        public long BusId { get; set; }
        public string DriverName { get; set; }
        public string DriverNumber { get; set; }
        public string DriverProfile { get; set; }
        public int TravellingStatus { get; set; }
        public List<TripTravel> Travel { get; set; }
    }
}
