namespace Satluj_Latest.Data
{
    public partial class SP_StudentCurrentTravellingData_Result
    {
        public SP_StudentCurrentTravellingData_Result()
        {
            
        }
        public long TripId { get; set; }
        public long DriverId { get; set; }
        public long SchoolId { get; set; }
        public string TripNo { get; set; }
        public System.DateTime TripDate { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public System.DateTime StartTime { get; set; }
        public System.DateTime ReachTime { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public System.Guid TripGuid { get; set; }
        public bool IsActive { get; set; }
        public int TravellingStatus { get; set; }
        public long BusId { get; set; }
        public Nullable<int> ShiftStatus { get; set; }
    }
}
