namespace Satluj_Latest.Models
{
    public class SPFullFees
    {
        public int FeeId { get; set; }
        public decimal Amount { get; set; }
        public Guid FeeGuid { get; set; }
        public string Feename { get; set; }
        public DateTime DueDate { get; set; }
        public int DiscountAllowed { get; set; }
        public int StudentspecialFee { get; set; }
        public int From { get; set; }
        public int BillNo { get; set; }

        public  SPFullFees() { }
    }
}
