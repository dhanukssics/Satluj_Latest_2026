using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPStudentCurrentTravellingData:BaseReference
    {
        public SPStudentCurrentTravellingData()
        {
            
        }
        private SP_StudentCurrentTravellingData_Result tripData;
        public SPStudentCurrentTravellingData(SP_StudentCurrentTravellingData_Result obj) { tripData = obj; }
        public long TripId { get { return tripData.TripId; } }
        public long DriverId { get { return tripData.DriverId; } }
        public long SchoolId { get { return tripData.SchoolId; } }
        public string TripNo { get { return tripData.TripNo; } }
        public DateTime TripDate { get { return tripData.TripDate; } }
        public string FromLocation { get { return tripData.FromLocation; } }
        public string ToLocation { get { return tripData.ToLocation; } }
        public DateTime StartTime { get { return tripData.StartTime; } }
        public DateTime ReachTime { get { return tripData.ReachTime; } }
        public DateTime TimeStamp { get { return tripData.TimeStamp; } }
        public Guid TripGuid { get { return tripData.TripGuid; } }
        public bool IsActive { get { return tripData.IsActive; } }
        public int TravellingStatus { get { return tripData.TravellingStatus; } }
        public long BusId { get { return tripData.BusId; } }
    }
}
