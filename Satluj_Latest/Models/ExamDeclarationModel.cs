using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ExamDeclarationModel
    {
        public long SchoolId { get; set; }
        public long TermId { get; set; }
        public long ClassId { get; set; }
        public string StartDateString { get; set; }
        public DateTime StartDate { get; set; }
        public string EndDateString { get; set; }
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Required")]
        public string ExamName { get; set; }
        public long Id { get; set; }
        public string TermName { get; set; }
        public string ClassName { get; set; }
    }
}