namespace Satluj_Latest.Models
{
    public class AddClassFees
    {
        public long SchoolId { get; set; }


        //[Required(ErrorMessage = "Required")]
        public string FeeId { get; set; }
        public string DataList { get; set; }
        public int Interval { get; set; }
        public DateTime DueDate { get; set; }

    }
}
