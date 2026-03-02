namespace Satluj_Latest.Models
{
    public partial class SP_CircularNotification
    {
        public SP_CircularNotification()
        {
            
        }
        public long CircularId { get; set; }
        public long SchoolId { get; set; }
        public int LoginType { get; set; }
        public Nullable<long> USerId { get; set; }
        public System.DateTime CircularDate { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime TimeStamp { get; set; }
    }
}
