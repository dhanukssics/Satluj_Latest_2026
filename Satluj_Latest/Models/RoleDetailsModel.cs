using Microsoft.AspNetCore.Mvc.Rendering;
using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class RoleDetailsModel
    {
        public long SchoolId { get; set; }
        public long Id { get; set; }
        [Required(ErrorMessage = "Role Name Required")]
        public string RoleName { get; set; }
        public bool IsAdmin { get; internal set; }
        public List<SelectListItem> RolesList { get; set; }
    }
}