using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class SubjectsModel
    {
        [Required(ErrorMessage = "Subject Name  Required")]
        public string SubjectName { get; set; }
        public long SchoolId { get; set; }
        public bool IsOptional { get; set; }
        public long SubjectId { get; set; }
        public string Abbreviation { get; set; }
        public string Code { get; set; }
    }
}