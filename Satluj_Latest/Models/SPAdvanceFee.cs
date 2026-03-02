using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{

    public class SPAdvanceFee : BaseReference
    {

        

        public long FeeId { get; set; }
        public string FeeName { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public Guid FeeGuid { get; set; }
    }
}
