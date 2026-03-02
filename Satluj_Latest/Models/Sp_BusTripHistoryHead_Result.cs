namespace Satluj_Latest.Models
{
    public partial class Sp_BusTripHistoryHead_Result
    {
        public Sp_BusTripHistoryHead_Result()
        {
            
        }
        public long BusId { get; set; }
        public string TripNo { get; set; }
        public Nullable<int> ShiftStatus { get; set; }
        public string BusName { get; set; }
        public string LocationStart { get; set; }
        public string LocationEnd { get; set; }
    }
}
