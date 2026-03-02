using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPBalanceSheet
    {
        public int TypeId { get; set; }
        public string Head { get; set; }
        public string Assets { get; set; }
        public decimal? AssetsAmount { get; set; }
        public string Liability { get; set; }
        public decimal? LiabilityAmount { get; set; }
    }
}
