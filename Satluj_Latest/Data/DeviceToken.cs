using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class DeviceToken:BaseReference
    {
        private TbDeviceToken deviceToken;
        public DeviceToken(TbDeviceToken obj) { deviceToken = obj; }
        public DeviceToken(long id) { deviceToken = _Entities.TbDeviceTokens.FirstOrDefault(z => z.TokenId == id); }
        public long TokenId { get { return deviceToken.TokenId; } }
        public int RoleId { get { return deviceToken.RoleId; } }
        public long UserId { get { return deviceToken.UserId; } }
        public string Token { get { return deviceToken.Token; } }
        public System.DateTime TimeStamp { get { return deviceToken.TimeStamp; } }
        public bool IsActive { get { return deviceToken.IsActive; } }
        public int LoginStatus { get { return deviceToken.LoginStatus; } } //1: Login 0:Logout

    }
}
