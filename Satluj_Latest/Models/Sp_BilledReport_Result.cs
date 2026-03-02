
namespace Satluj_Latest.Models
{
    public class Sp_BilledReport_Result
    {
        public long StudentId { get; set; }
        public long FeeId { get; set; }
        public decimal? Amount { get; set; }
        public Guid? FeeGuid { get; set; }
        public string Feename { get; set; }
        public DateTime DueDate { get; set; }
        public int DiscountAllowed { get; set; }
        public int StudentspecialFee { get; set; }
    }
}
