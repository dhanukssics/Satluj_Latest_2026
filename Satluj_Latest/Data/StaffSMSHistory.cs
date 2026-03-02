using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class StaffSMSHistory:BaseReference
    {
        private TbStaffSmshistory staffSMSHistory;
        public StaffSMSHistory(TbStaffSmshistory obj) { staffSMSHistory = obj; }
        public StaffSMSHistory(long Id) { staffSMSHistory = _Entities.TbStaffSmshistories.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return staffSMSHistory.Id; } }
        public long StaffId { get { return staffSMSHistory.Id; } }
        public string MessageContent { get { return staffSMSHistory.MessageContent; } }
        public string DelivaryStatus { get { return staffSMSHistory.DelivaryStatus; } }
        public DateTime? MessageDate { get { return staffSMSHistory.MessageDate; } }
        public bool IsActive { get { return staffSMSHistory.IsActive; } }
        public string SendStatus { get { return staffSMSHistory.SendStatus; } }
        public long ScholId { get { return staffSMSHistory.ScholId; } }
        public string MobileNumber { get { return staffSMSHistory.MobileNumber; } }
        public int? SmsSentPerStudent { get { return staffSMSHistory.SmsSentPerStudent; } }
        public long HeadId { get { return staffSMSHistory.HeadId; } }
        public Nullable<int> UserType { get { return staffSMSHistory.UserType; } }
        public string Name { get { return staffSMSHistory.Staff.Name; } }
       
        public string ContactNumber { get { return GetContactNumber(staffSMSHistory); } }
        public string MessageReturnId { get { return staffSMSHistory.MessageReturnId; } }
        private string GetContactNumber(TbStaffSmshistory sms)
        {
            string ContactNumber = "";

            if (sms.UserType == 1)
                ContactNumber = sms.Staff.TbTeachers.FirstOrDefault(x => x.UserId == sms.StaffId).ContactNumber;
            else
                ContactNumber = sms.Staff.TbStaffs.FirstOrDefault(x => x.UserId == sms.StaffId).Contact;
            return ContactNumber;
        }

    }
}
