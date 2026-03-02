using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class CalendarEventModels
    {
        public long eventId { get; set; }

        [Required(ErrorMessage = "Required")]
        public long schoolId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string eventHead { get; set; }
        [Required(ErrorMessage = "Required")]
        public string eventDetails { get; set; }
        [Required(ErrorMessage = "Required")]
        public string eventDate { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

    }
}