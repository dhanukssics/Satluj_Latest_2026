using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class SMSHistoryModel
    {
        public long headId { get; set; }
        public string head { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsActive { get; set; }
        public int? SenderType { get; set; }
    }
}