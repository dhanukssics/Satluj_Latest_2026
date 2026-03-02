using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class LibraryModels
    {
        public long categoryId { get; set; }
        public long bookId { get; set; }
        public long schoolId { get; set; }
        public long studentId { get; set; }
        public long bookCount { get; set; }

        [Required(ErrorMessage = "Required")]
        public string admissionNumber { get; set; }
        [Required(ErrorMessage = "Required")]
        public string categoryName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string title { get; set; }
        [Required(ErrorMessage = "Required")]
        public string author { get; set; }
        public int status { get; set; }
        public bool isActive { get; set; }
        public string ReferenceNumber { get; set; }
    }
}

