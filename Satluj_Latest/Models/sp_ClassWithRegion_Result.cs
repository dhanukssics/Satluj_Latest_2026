namespace Satluj_Latest.Models
{
    public partial class sp_ClassWithRegion_Result
    {
        public sp_ClassWithRegion_Result()
        {
            
        }
        //public long Id { get; set; }
        //public long Region { get; set; }
        //public long ClassId { get; set; }
        //public bool IsActive { get; set; }
        //public System.DateTime TimeStamp { get; set; }
        //public string Class { get; set; }
        //public string RegionName { get; set; }
        public long Id { get; set; }

        public long RegionId { get; set; }

        public long Region
        {
            get { return RegionId; }
            set { RegionId = value; }
        }

        public long ClassId { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public string Class { get; set; }
        public string RegionName { get; set; }

    }
}
