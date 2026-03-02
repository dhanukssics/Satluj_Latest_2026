namespace Satluj_Latest.Models
{
    public partial class sp_StudentMarkPercentage_Result
    {
        public long StudentId { get; set; }
        public long SubjectId { get; set; }
        public long DivisionId { get; set; }
        public Nullable<decimal> Percentage { get; set; }
    }

}
