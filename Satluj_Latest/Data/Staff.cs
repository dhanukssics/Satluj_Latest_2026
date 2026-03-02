using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Staff : BaseReference
    {
        private TbStaff staff;
        public Staff(TbStaff obj) { staff = obj; }
        //public Staff(long id) { staff = _Entities.TbStaffs.FirstOrDefault(z => z.StaffId == id); }
        public Staff(long id)
        {
            staff = _Entities.TbStaffs
                .Include(x => x.User)
                .FirstOrDefault(x => x.StaffId == id);
        }
        public long StaffId { get { return staff.StaffId; } }
        public long UserId { get { return staff.UserId; } }
        public string StaffName { get { return staff.StaffName; } }
        public string Contact { get { return staff.Contact; } }
        public string Address { get { return staff.Address; } }
        public System.DateTime DOB { get { return staff.Dob; } }
        public string EmailId { get { return staff.User.Username; } }
        public string Password { get { return staff.User.Password; } }
        public bool IsActive { get { return staff.IsActive; } }
        public System.DateTime TimeStamp { get { return staff.TimeStamp; } }
        public Nullable<long> DesignationId { get { return staff.DesignationId; } }
        public Nullable<long> DepartmentId { get { return staff.DepartmentId; } }
        public Nullable<long> UserType { get { return staff.UserType; } }

        public TbStaff X { get; }

        public List<UserRoleDetails> GetUserRoleDetails()
        {
            List<UserRoleDetails> userRoles = new List<UserRoleDetails>();
            var userRole = _Entities.TbUserRoles.Where(x => x.UserId == staff.UserId && x.IsActive).ToList();
            if (userRole.Count > 0 && userRole != null)
                userRoles = userRole.Select(x => new UserRoleDetails(x)).ToList();
            return userRoles;
        }
        public string RolesString()
        {
            string roles = "";
            var list = GetUserRoleDetails();
            if (list != null)
                roles = String.Join(",", from item in list select item.RoleName);
            return roles;
        }
        public string Department()
        {
            string department = "";
            long teacherId = staff.UserId;
            var dep = staff.Department;
            if (dep != null)
                department = dep.DepartmentName;
            return department;
        }
        public string Designation()
        {
            string designation = "";
            long teacherId = staff.UserId;
            var des = staff.Designation;
            if (des != null)
                designation = des.DesignationName;
            return designation;
        }
        public string RoleIdString()
        {
            string roles = "";
            var list = GetUserRoleDetails();
            if (list != null)
                roles = String.Join("~", from item in list select item.RoleId);
            return roles;
        }
        public Staff(long id, int status) { staff = _Entities.TbStaffs.Where(z => z.UserId == id).FirstOrDefault(); }

       

        public List<UserModule> GetStaffModules()
        {
            List<UserModule> list = new List<UserModule>();
            if (staff.UserType != null)
            {
                var data = _Entities.TbUserModuleDetails.Where(c => c.UserModuleId == staff.UserType && c.IsActive).ToList();
                foreach (var item in data)
                {
                    UserModule one = new UserModule();
                    one.MainId = item.MainId;
                    one.SubId = item.SubModuleId;
                    list.Add(one);
                }
            }
            return list;
        }
    }
}
