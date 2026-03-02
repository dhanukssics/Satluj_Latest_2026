using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Bus:BaseReference
    {
        private TbBu bus;
        public Bus(TbBu obj) { bus = obj; }
        public Bus(long id) { bus = _Entities.TbBus.FirstOrDefault(z => z.BusId == id); }
        public long BusId { get { return bus.BusId; } }
        public string BusSpecialId { get { return bus.BusSpecialId; } }
        public int TripNumber { get { return bus.TripNumber; } }
        public string LocationStart { get { return bus.LocationStart; } }
        public string LocationEnd { get { return bus.LocationEnd; } }
        public System.DateTime TimeStamp { get { return bus.TimeStamp; } }
        public System.Guid BusGuid { get { return bus.BusGuid; } }
        public bool IsActive { get { return bus.IsActive; } }
        public string BusType { get { return bus.BusType; } }
        public string BusName { get { return bus.BusName; } }
        public long SchoolId { get { return bus.SchoolId; } }
        public int TravellingStatus { get { return GetTripDetails(bus.BusId); } }
        public int GetTripDetails(long BusId)
        {
         //var tripnew= bus.tb_Trip.Where(z => z.IsActive && z.BusId==BusId && z.TimeStamp.Date==DateTime.UtcNow.Date).OrderByDescending(z=>z.TripId).ToList().Select(z => new Trip(z)).FirstOrDefault();
         var tripnew= bus.TbTrips.Where(z => z.IsActive && z.BusId==BusId ).OrderByDescending(z=>z.TripId).ToList().Select(z => new Trip(z)).FirstOrDefault();
            if(tripnew==null)
            {
                return 3;
            }
            else
            {
                return tripnew.TravellingStatus; 
            }
        
        }
    }

   
}
