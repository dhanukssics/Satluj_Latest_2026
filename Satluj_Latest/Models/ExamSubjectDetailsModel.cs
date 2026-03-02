using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ExamSubjectDetailsModel
    {
        public long ExamId { get; set; }
        [Required(ErrorMessage = "Subject Required")]
        public long SubjectId { get; set; }
        public long ExamSubjectId { get; set; }

        public string Subject { get; set; }

        //[RegularExpression("[^0-9]", ErrorMessage = "Internal Mark must be numeric")]
        [Required(ErrorMessage = "Internal Mark Required")]
        public decimal Internal { get; set; }


        //[RegularExpression("[^0-9]", ErrorMessage = "External Mark must be numeric")]
        [Required(ErrorMessage = "External Mark Required")]
        public decimal External { get; set; }
        [Required(ErrorMessage = "Total Mark Required")]
        public decimal Total { get; set; }
        public DateTime ExamDate { get; set; }
        public long SchoolId { get; set; }

        [Required(ErrorMessage = "Required")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{HH:mm:ss}")]
        //[RegularExpression(@"^(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "     Invalid time.")]
        [RegularExpression(@"^(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$", ErrorMessage = "     Invalid time.")]
        public TimeSpan ExamTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ExamName { get; set; }
    }
}

