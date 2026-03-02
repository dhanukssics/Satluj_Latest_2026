using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPStudentBillPayment:BaseReference 
    {
        
         public string FeesName { get; set; }
         public decimal Amount { get; set; }
        public long FeeId { get; set; }
        public decimal Discount { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
