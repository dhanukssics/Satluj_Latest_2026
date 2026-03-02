using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SpMessageForParent : BaseReference
    {
        public SpMessageForParent()
        {
            
        }
        private SpParentTeacherConversationFullResult message;
        public SpMessageForParent(SpParentTeacherConversationFullResult obj) { message = obj; }
        public long messageId { get { return message.MessageId; } }
        public long senderId { get { return message.SenderId; } }
        public long studentId { get { return message.StudentId; } }
        public string subject { get { return message.Subject; } }
        public string description { get { return message.Description; } }
        public string teacherName
        {
            get
            {
                if (message.Status == 1)
                {
                    return _Entities.TbTeachers.Where(z => z.TeacherId == message.SenderId).FirstOrDefault().TeacherName;
                }
                else
                {
                    return "";
                }
            }
        }

        public string filePath { get { return message.FilePath; } }
        public DateTime TimeStamp { get { return message.TimeStamp; } }
        public string role { get { return message.Role; } }
        public int status { get { return message.Status; } }

    }
}
