using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Models
{
    public class SPPush_StudentMessage
    {
        public SPPush_StudentMessage()
        {
            
        }
        private SP_Push_StudentMessage_Result studentMessage;
          public SPPush_StudentMessage(SP_Push_StudentMessage_Result obj) { studentMessage = obj; }
          public long TokenId { get { return studentMessage.TokenId; } }
          public int RoleId { get { return studentMessage.RoleId; } }
          public long UserId { get { return studentMessage.UserId; } }
          public string Token { get { return studentMessage.Token; } }
          public DateTime TimeStamp { get { return studentMessage.TimeStamp; } }
          public bool IsActive { get { return studentMessage.IsActive; } }
          public int LoginStatus { get { return studentMessage.LoginStatus; } }
    }
}
