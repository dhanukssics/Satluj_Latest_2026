using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class AddClassModel
    {
        public long SchoolId { get; set; }
        [Required(ErrorMessage = "Required")]
        public long AcademicYearId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ClassName { get; set; }
        public int OrderValue { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Invalid")]
        public string Division { get; set; }
        public IEnumerable<SelectListItem> ClassList { get; set; }
        public List<SelectListItem> currentacademiclist { get; set; }

    }
}