using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
   public class SmsHead: BaseReference
    {

        private TbSmsHead sms;
        public SmsHead(TbSmsHead obj) { sms = obj; }
        public SmsHead(long id) { sms = _Entities.TbSmsHeads.FirstOrDefault(z => z.HeadId == id); }
        public long headId { get { return sms.HeadId; } }
        public string head { get { return sms.Head; } }
        public DateTime TimeStamp { get { return sms.TimeStamp; } }
        public bool IsActive { get { return sms.IsActive; } }
        public List<SmsHistory> SmsHistory { get { return sms.TbSmsHistories.ToList().Select(z => new SmsHistory(z)).ToList(); } }
        public List<StaffSMSHistory> StaffSMSHistory { get { return sms.TbStaffSmshistories.ToList().Select(z => new StaffSMSHistory(z)).ToList(); } }
        public int? SenderType { get { return sms.SenderType; } }

       
    }

}
