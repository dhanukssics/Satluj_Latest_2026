namespace Satluj_Latest.Models
{
    public partial class sp_StudentMarkList_Result
    {
        public sp_StudentMarkList_Result()
        {
            
        }
        public string StundentName { get; set; }
        public decimal StudentExternalMark { get; set; }
        public decimal StudentInternalMark { get; set; }
        public long StudentTotalMark { get; set; }
        public long ExamId { get; set; }
        public long SubId { get; set; }
        public string Subject { get; set; }
        public Nullable<long> StudentId { get; set; }
        public decimal TotalMark { get; set; }
        public decimal TotalInternalMark { get; set; }
        public decimal TotalExternalMark { get; set; }
    }
}
