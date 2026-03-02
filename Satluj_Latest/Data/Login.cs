using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Login:BaseReference
    {
         private TbLogin login;
         public Login(TbLogin obj) { login = obj; }
         public Login(long id) { login = _Entities.TbLogins.FirstOrDefault(z => z.UserId == id); }
         public long UserId { get { return login.UserId; } }
         public long SchoolId { get { return login.SchoolId; } }
         public int RoleId { get { return login.RoleId; } }
         public string Name { get { return login.Name; } }
         public string Username { get { return login.Username; } }
         public string Password { get { return login.Password; } }
         public bool IsActive { get { return login.IsActive; } }
         public System.DateTime TimeStamp { get { return login.TimeStamp; } }
         public bool DisableStatus { get { return login.DisableStatus; } }
         public System.Guid LoginGuid { get { return login.LoginGuid; } }
         public School School { get { return new School(login.School); } }


        public List<TbUserAllotedMenu> GetUserMenuList()
        {
            var data = login.TbUserAllotedMenus.ToList().OrderBy(z=>z.Menu.OrderValue).ToList();
            return data;
        }
    }
}
