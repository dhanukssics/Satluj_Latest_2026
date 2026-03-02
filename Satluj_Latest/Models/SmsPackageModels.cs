using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class SmsPackageModels
    {
        public long PackageId { get; set; }
        public long SchoolId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public long AllowedSms { get; set; }
        public Decimal SmsRate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDisabled { get; set; }
        public bool SmsStatus { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}