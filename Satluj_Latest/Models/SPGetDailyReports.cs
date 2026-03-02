using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPGetDailyReports:BaseReference
    {
        public long StudentId { get; set; }
        public long? BillNo { get; set; }
        public long ClassId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? Date { get; set; }


    }
}
