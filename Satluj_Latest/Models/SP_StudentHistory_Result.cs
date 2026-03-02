namespace Satluj_Latest.Models
{
    public partial class SP_StudentHistory_Result
    {
        public SP_StudentHistory_Result()
        {
            
        }
        public long FeeId { get; set; }
        public decimal Amount { get; set; }
        public System.Guid FeeGuid { get; set; }
        public string Feename { get; set; }
        public System.DateTime DueDate { get; set; }
    }
}