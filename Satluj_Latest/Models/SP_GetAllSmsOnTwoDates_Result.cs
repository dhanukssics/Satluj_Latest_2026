namespace Satluj_Latest.Models
{
    public partial class SP_GetAllSmsOnTwoDates_Result
    {
        public SP_GetAllSmsOnTwoDates_Result()
        {
            
        }
        public long Id { get; set; }
        public long StuentId { get; set; }
        public string MessageContent { get; set; }
        public string DelivaryStatus { get; set; }
        public Nullable<System.DateTime> MessageDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string SendStatus { get; set; }
        public long ScholId { get; set; }
        public string MobileNumber { get; set; }
        public string Division { get; set; }
        public string StundentName { get; set; }
        public string Class { get; set; }
        public Nullable<int> SmsSentPerStudent { get; set; }
    }
}
