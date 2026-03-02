namespace Satluj_Latest.Models
{
    public partial class sp_FeeAlertDetails
    {
        public sp_FeeAlertDetails()
        {
            
        }
        public Nullable<decimal> Amount { get; set; }
        public string StundentName { get; set; }
        public long SchoolId { get; set; }
        public long StudentId { get; set; }
        public long ClassId { get; set; }
        public string ContactNumber { get; set; }
        public string FeesName { get; set; }
        public int StatusFeild { get; set; }
    }
}

