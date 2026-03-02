using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class AddCircularNotification
    {
        public long CircularId { get; set; }
        public DateTime DocumentDate { get; set; }
        [Required(ErrorMessage = "Date  Required")]

        public string DocumentDateString { get; set; }

        [Required(ErrorMessage = "DocumentDetails Required")]
        public string DocumentDetails { get; set; }
        [Required(ErrorMessage = "DocumentDetails Required")]
        public string DocumentHead { get; set; }
        [Required(ErrorMessage = "DocumentHead Required")]
        public string FilePath { get; set; }
        
    }
}