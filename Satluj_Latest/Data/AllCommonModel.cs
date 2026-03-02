using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class AllCommonModel
    {
    }
    
    public class DetailedReport
    {
        public int SlNo { get; set; }
        public string BillNo { get; set; }
        public DateTime BillDate { get; set; }
        public string StudentName { get; set; }
        public string Class { get; set; }
        public string Particulars { get; set; }
        public decimal Amount { get; set; }
    }
}
