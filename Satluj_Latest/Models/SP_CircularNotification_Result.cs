namespace Satluj_Latest.Models
{
    public partial class SP_CircularNotification_Result
    {
        public SP_CircularNotification_Result()
        {
            
        }
        public long CircularId { get; set; }
        public long SchoolId { get; set; }
        public int LoginType { get; set; }
        public long? USerId { get; set; }
        public DateTime CircularDate { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public bool IsActive { get; set; }
        public DateTime TimeStamp { get; set; }
    }

}
