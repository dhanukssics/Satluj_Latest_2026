using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class UserRoleDetails : BaseReference
    {
        private TbUserRole userRole;
        public UserRoleDetails(TbUserRole obj) { userRole = obj; }
        public UserRoleDetails(long id) { userRole = _Entities.TbUserRoles.FirstOrDefault(z => z.Id == id); }

        //public UserRoleDetails(TbUserRole x)
        //{
        //    X = x;
        //}

        public long Id { get { return userRole.Id; } }
        public long UserId { get { return userRole.UserId; } }
        public long RoleId { get { return userRole.RoleId; } }
        public bool IsActive { get { return userRole.IsActive; } }
        public System.DateTime TimeStamp { get { return userRole.TimeStamp; } }
        public string RoleName { get { return userRole.Role.RoleName; } }

        public TbUserRole X { get; }
    }
}
