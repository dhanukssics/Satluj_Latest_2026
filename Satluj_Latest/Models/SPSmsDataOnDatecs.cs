using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPSmsDataOnDatecs:BaseReference
    {
        public SPSmsDataOnDatecs()
        {
            
        }
        private SP_GetAllSmsOnTwoDates_Result msg;
        public SPSmsDataOnDatecs(SP_GetAllSmsOnTwoDates_Result obj) { msg = obj; }
        public long Id { get { return msg.Id; } }
        public long StuentId { get { return msg.StuentId; } }
        public string MessageContent { get { return msg.MessageContent; } }
        public string DelivaryStatus { get { return msg.DelivaryStatus; } }
        public DateTime? MessageDate { get { return msg.MessageDate; } }
        public bool? IsActive { get { return msg.IsActive; } }
        public string SendStatus { get { return msg.SendStatus; } }
        public long ScholId { get { return msg.ScholId; } }
        public string MobileNumber { get { return msg.MobileNumber; } }

        public string StuedentName { get { return msg.StundentName; } }
        public string StudentDivision { get { return msg.Division; } }
        public string StudentClass { get { return msg.Class; } }
        public int? SmsSentPerStudent { get { return msg.SmsSentPerStudent; } }
    }
}
