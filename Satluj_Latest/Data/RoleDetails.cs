using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class RoleDetails : BaseReference
    {
        private TbRoleDetail roleDetails;
        public RoleDetails(TbRoleDetail obj) { roleDetails = obj; }
        public RoleDetails(long Id) { roleDetails = _Entities.TbRoleDetails.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return roleDetails.Id; } }
        public long SchoolId { get { return roleDetails.SchoolId; } }
        public string RoleName { get { return roleDetails.RoleName; } }
        public bool IsActive { get { return roleDetails.IsActive; } }
        public System.DateTime TimeStamp { get { return roleDetails.TimeStamp; } }
    }
}
