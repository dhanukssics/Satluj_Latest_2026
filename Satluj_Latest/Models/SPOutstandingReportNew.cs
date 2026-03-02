using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SP_OutstandingReportNew_Result
    {
        private SP_OutstandingReportNew_Result x;

        public SP_OutstandingReportNew_Result()
        {

        }
        public SP_OutstandingReportNew_Result(SP_OutstandingReportNew_Result x)
        {
            this.x = x;
        }

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
