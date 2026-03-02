using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Message:BaseReference
    {
          private TbMessage message;
          public Message(TbMessage obj) { message = obj; }
          public Message(long id) { message = _Entities.TbMessages.FirstOrDefault(z => z.MessageId == id); }
          public long MessageId { get { return message.MessageId; } }
          public long TeacherId { get { return message.TeacherId; } }
          public long StudentId { get { return message.StudentId; } }
          public string Subject { get { return message.Subject; } }
          public string Descrption { get { return message.Descrption; } }
          public bool MessageType { get { return message.MessageType; } }  // true :Single student message ,false : Multiple student message
          public System.DateTime TimeStamp { get { return message.TimeStamp; } }
          public bool IsActive { get { return message.IsActive; } }
          public string teacherName { get { return message.Teacher.TeacherName; } }
          public string teacherContact { get { return message.Teacher.ContactNumber; } }


    }
}
