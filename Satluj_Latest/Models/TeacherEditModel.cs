using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;


namespace Satluj_Latest.Models
{
   
    public class TeacherEditModel
    {
        public long schoolId { get; set; }
        public long teacherId { get; set; }

        [Required]
        public string teacherName { get; set; }

        public string classId { get; set; }
        public string divisionId { get; set; }

        [Required]
        public string contactNumber { get; set; }

        [Required, EmailAddress]
        public string emailId { get; set; }

        public long DepartmentId { get; set; }
        public long DesignationId { get; set; }

        public string RolesData { get; set; }

      
        public IEnumerable<SelectListItem> DesignationList { get; set; }
        public IEnumerable<SelectListItem> DepartmentList { get; set; }

        public IEnumerable<SelectListItem> UserTypes { get; set; }
        public List<RoleVM> RolesList { get; set; } = new();
        public long? UserTypeId { get; set; }
    }

}
