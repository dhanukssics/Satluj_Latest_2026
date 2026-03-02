using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Trip : BaseReference
    {
        private TbTrip trip;
        public Trip(TbTrip obj) { trip = obj; }
        public Trip(long id) { trip = _Entities.TbTrips.FirstOrDefault(z => z.TripId == id); }
        public long TripId { get { return trip.TripId; } }
        public long DriverId { get { return trip.DriverId; } }
        public long SchoolId { get { return trip.SchoolId; } }
        public string TripNumber { get { return trip.TripNo; } }
        public string LocationStart { get { return trip.FromLocation; } }
        public string LocationEnd { get { return trip.ToLocation; } }
        public System.DateTime StartTime { get { return trip.StartTime; } }
        public System.DateTime? ReachTime { get { return trip.ReachTime; } }
        public System.DateTime TimeStamp { get { return trip.TimeStamp; } }

        public System.Guid TripGuid { get { return trip.TripGuid; } }
        public bool IsActive { get { return trip.IsActive; } }
        public int TravellingStatus { get { return trip.TravellingStatus; } }
        public long BusId { get { return trip.BusId; } }
        public string BusName { get { return trip.Bus.BusName; } }
        public Nullable<int> ShiftStatus { get { return trip.ShiftStatus; } }
        public string DriverName { get { return trip.Driver.DriverName; } }

        public string DriverNumber { get { return trip.Driver.ContactNumber; } }
        public string DriverProfile { get { return trip.Driver.FilePath; } }
        public Driver Driver { get { return new Data.Driver(trip.Driver); } }
        public List<Travel> Travel { get { return trip.TbTravels.ToList().Select(z => new Travel(z)).ToList(); } }

        public string LastLocation
        {
            get
            {
                if (trip.TbTravels.ToList().Count > 0)
                {
                    return trip.TbTravels.ToList().OrderByDescending(z => z.TravelId).FirstOrDefault().Place;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}

