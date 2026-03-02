using Microsoft.AspNetCore.Mvc.Rendering;
using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class StaffModels
    {
        public long SchoolId { get; set; }
        public long UserId { get; set; }
        public long staffId { get; set; }

        public long?  UserTypeId { get; set; }

        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Entered e-mail is not a valid mail")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Entered e-mail is not a valid mail")]
        public string emailId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Contact { get; set; }
        [Required(ErrorMessage = "Required")]
        public string DOBstring { get; set; }
        public int userType { get; set; }

        public string type { get; set; }
        public long DepartmentId { get; set; }
        public long DesignationId { get; set; }
        public string RoleData { get; set; }
        public IEnumerable<SelectListItem> Designations { get; set; }
        public IEnumerable<SelectListItem> Departments { get; set; }
        public IEnumerable<SelectListItem> UserTypes { get; set; }
        public List<SelectListItem> RolesList { get; set; }

    }
}