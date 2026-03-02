namespace Satluj_Latest.Models
{
    public class Sp_GetAllSmsCount
    {
        public long Id { get; set; }
        public long StuentId { get; set; }
        public string MessageContent { get; set; }
        public string DelivaryStatus { get; set; }
        public DateTime MessageDate { get; set; }
        public bool IsActive { get; set; }
        public string SendStatus { get; set; }
        public long ScholId { get; set; }
        public string MobileNumber { get; set; }
        public string Division { get; set; }
        public string StundentName { get; set; }
        public string Class { get; set; }
        public int SmsSentPerStudent { get; set; }

        // EXTRA PROPERTIES for your view
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public int Count { get; set; }
    }
}
