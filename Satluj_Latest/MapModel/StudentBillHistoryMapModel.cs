using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class StudentBillHistoryMapModel
    {
        public  List<PaidHistory> PaidHistoryData {get;set;}
        public List<DueBills> DueHistoryData { get; set; }
    }
    public class PaidHistory
    {
        public DateTime Billdate { get; set; }
        public long BillNo { get; set; }
        public List<PaidBills> PaidBillsData { get; set; }
    }
    //public class DueHistory
    //{
    //    public DateTime Billdate { get; set; }
    //    public long BillNo { get; set; }
    //    public List<DueBills> DueBillsData { get; set; }
    //}
    public class PaidBills
    {
        public string Particulars { get; set; }
        public decimal Discount { get; set; }
        public decimal Amount { get; set; }
    }
    public class DueBills
    {
        public string Particulars { get; set; }
        public string Details { get; set; }
        public decimal Amount { get; set; }
    }
}
