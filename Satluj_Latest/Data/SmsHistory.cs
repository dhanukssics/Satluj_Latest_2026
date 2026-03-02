using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class SmsHistory : BaseReference
    {
        private readonly TbSmsHistory _sms;

        public SmsHistory(TbSmsHistory obj)
        {
            _sms = obj ?? throw new ArgumentNullException(nameof(obj));
        }

       
        public SmsHistory(long id)
        {
            _sms = _Entities.TbSmsHistories
                .Include(x => x.Stuent)
                .ThenInclude(s => s.Class)
                .Include(x => x.Stuent.Division)
                .FirstOrDefault(x => x.Id == id)
                ?? throw new Exception($"Sms record with ID {id} not found.");
        }

        public long Id => _sms.Id;
        public long StudentId => _sms.StuentId;
        public string MessageContent => _sms.MessageContent;
        public string DeliveryStatus => _sms.DelivaryStatus;
        public DateTime? MessageDate => _sms.MessageDate;
        public bool? IsActive => _sms.IsActive;
        public string SendStatus => _sms.SendStatus;

        public string StudentName => _sms.Stuent?.StundentName ?? "Unknown";
        public string StudentDivision => _sms.Stuent?.Division?.Division ?? "-";
        public string StudentClass => _sms.Stuent?.Class?.Class ?? "-";

        public int? SmsPerStudent => _sms.SmsSentPerStudent;
        public string MessageReturnId => _sms.MessageReturnId;
    }

}
