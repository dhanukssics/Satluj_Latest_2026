using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ParentMessage:BaseReference
    {
        private TbParentMessage msg;
        public ParentMessage(TbParentMessage obj) { msg = obj; }
        public ParentMessage(long Id) { msg = _Entities.TbParentMessages.FirstOrDefault(z => z.MessageId == Id); }
        public long MessageId { get { return msg.MessageId; } }
        public long SenderId { get { return msg.SenderId; } }
        public long StudentId { get { return msg.StudentId; } }
        public string Subject { get { return msg.Subject; } }
        public string Description { get { return msg.Description; } }
        public string FilePath { get { return msg.FilePath; } }
        public bool IsActive { get { return msg.IsActive; } }
        public System.DateTime TimeStamp { get { return msg.TimeStamp; } }

        public string parentNamr { get { return msg.Sender.ParentName; } }
        public string studentName { get { return msg.Student.StundentName; } }
        public bool ReadStatus { get { return msg.ReadStatus; } } //1: New Message, 0: Road Message
    }
}
