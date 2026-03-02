using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ExamsModel
    {
        public long SchoolId { get; set; }
        public long UserId { get; set; }
        [Required(ErrorMessage = "Class Required")]
        public long ClassId { get; set; }
        [Required(ErrorMessage = "Exam Name Required")]
        public string ExamName { get; set; }
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Exam Start Date Required")]
        public string StartDateString { get; set; }
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Exam End Date Required")]
        public string EndDateString { get; set; }
        public long ExamId { get; set; }

    }
}