namespace Satluj_Latest.Models
{
    public class ListFee
    {
        public long SchoolId { get; set; }
        public List<TbFee> Fees { get; set; } = new();
    }

}
