namespace Satluj_Latest.Models
{
    public partial class SP_GetSmsPackage_Result
    {
        public SP_GetSmsPackage_Result()
        {
            
        }
        public long PackageId { get; set; }
        public long SchoolId { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public long AllowedSms { get; set; }
        public decimal SmsRate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDisabled { get; set; }
    }
}
