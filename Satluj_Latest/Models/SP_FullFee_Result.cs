namespace Satluj_Latest.Models
{
    public partial class SP_FullFee_Result
    {
        public SP_FullFee_Result()
        {
            
        }
        public long FeeId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<System.Guid> FeeGuid { get; set; }
        public string Feename { get; set; }
        public System.DateTime DueDate { get; set; }
        public int DiscountAllowed { get; set; }
        public int StudentspecialFee { get; set; }
        public int From { get; set; }
        public Nullable<long> BillNo { get; set; }
    }
}
