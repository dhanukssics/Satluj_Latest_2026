namespace Satluj_Latest.Models
{
    public partial class sp_ReceiptPayment_Result
    {
        public sp_ReceiptPayment_Result()
        {
            
        }
        public string AccHeadName { get; set; }
        public int FromData { get; set; }
        public long Id { get; set; }
        public Nullable<int> BillNo { get; set; }
        public Nullable<decimal> Receipt { get; set; }
        public Nullable<decimal> Payment { get; set; }
    }
}
