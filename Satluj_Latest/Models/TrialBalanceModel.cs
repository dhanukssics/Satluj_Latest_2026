
namespace Satluj_Latest.Models
{
    public class TrialBalanceModel
    {
        public DateTime Today { get; set; }
        public DateTime StartDate { get; set; }
        public long SchoolId { get; set; }
        public List<sp_TrialBalance_Result> TrialBalanceList { get; internal set; }
    }
}
