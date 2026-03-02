using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Teacher : BaseReference
    {
        private readonly TbTeacher teacher;
        private long teacherId;

        public Teacher(TbTeacher obj)
        {
            teacher = obj;
        }

        public long TeacherId => teacher.TeacherId;
        public string TeacherName => teacher.TeacherName;
        
        public string TeacherSpecialId { get { return teacher.TeacherSpecialId; } }
        
        public long SchoolId { get { return teacher.SchoolId; } }
        public string ContactNumber { get { return teacher.ContactNumber; } }
        public string Email { get { return teacher.Email; } }
        public System.DateTime TimeStamp { get { return teacher.TimeStamp; } }
        public System.Guid TeacherGuid { get { return teacher.TeacherGuid; } }
        public bool IsActive { get { return teacher.IsActive; } }
        public string FilePath { get { return teacher.FilePath; } }
        public long DesignationId { get { return teacher.DesignationId ?? 0; } }
        public long DepartmentId { get { return teacher.DepartmentId ?? 0; } }
        public Nullable<long> UserType { get { return teacher.UserType ?? 0; } }
        public School SchoolData { get { return new School(teacher.School); } }
        public List<TeacherClass> TeacherClass { get { return GetTeacherClassData(teacher); } }
        public List<TeacherClass> GetTeacherClassData(TbTeacher teacher)
        {
            var teacherData = _Entities.TbTeacherClasses
                .Where(x => x.TeacherId == teacher.TeacherId)
                .Include(x => x.Class)
                .Include(x => x.Division)
                .ToList()
                .Select(x => new TeacherClass(x))
                .ToList();
            return teacherData;
        }
        public List<UserRoleDetails> GetUserRoleDetails()
        {
            List<UserRoleDetails> userRoles = new List<UserRoleDetails>();
            var userRole = _Entities.TbUserRoles.Where(x => x.UserId == teacher.UserId && x.IsActive).ToList();
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
            long teacherId = teacher.UserId;
            var dep = teacher.Department;
            if (dep != null)
                department = dep.DepartmentName;
            return department;
        }
        public string Designation()
        {
            string designation = "";
            long teacherId = teacher.UserId;
            var des = teacher.Designation;
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
        public Teacher(long id, int status) { teacher = _Entities.TbTeachers.Where(z => z.UserId == id).FirstOrDefault(); }

        public Teacher(long teacherId)
        {
            this.teacherId = teacherId;
        }

        public List<UserModule> GetTearhersModules()
        {
            List<UserModule> list = new List<UserModule>();
            if (teacher.UserType != null)
            {
                var data = _Entities.TbUserModuleDetails.Where(c => c.UserModuleId == teacher.UserType && c.IsActive).ToList();
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
        public List<EventsList> EventList()
        {
            List<EventsList> list = new List<EventsList>();
            var listOfEvents = new Satluj_Latest.Data.School(teacher.SchoolId).GetCalendarUpcomingEvent();
            foreach (var even in listOfEvents)
            {
                EventsList one = new EventsList();
                one.EventId = even.EventId;
                one.EventHead = even.EventHead;
                one.EventDate = even.EventDate;
                one.Description = even.EventDetails;
                list.Add(one);
            }
            return list.OrderByDescending(x => x.EventDate).ToList();
        }

        internal List<CircularsList> CircularList()
        {
            List<CircularsList> list = new List<CircularsList>();
            var listOfEvents = new Satluj_Latest.Data.School(teacher.SchoolId).AllCircularList();
            foreach (var cir in listOfEvents)
            {
                CircularsList one = new CircularsList();
                one.CircularId = cir.CircularId;
                one.FilePath = cir.FilePath;
                one.CircularDate = cir.CircularDate;
                one.Description = cir.Description;
                list.Add(one);
            }
            return list.OrderByDescending(x => x.CircularDate).ToList();
        }
    }
}
