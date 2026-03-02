using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class DepartmentsModel
    {
        public long SchoolId { get; set; }
        [Required(ErrorMessage = "Department Name Required")]
        public string DepartmentName { get; set; }
        [Required(ErrorMessage = "Abbreviation Required")]
        public string Abbreviation { get; set; }
        public long DepartmentId { get; set; }
    }
}