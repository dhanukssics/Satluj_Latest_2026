using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class MessageModels
    {
        public long UserId { get; set; }
        public string StudentsId { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public long ClassId { get; set; }
        public long SchoolId { get; set; }
        public long MessageId { get; set; }
        public int LockStatus { get; set; }
        public long DivisionId { get; set; }
        public string ClassName { get; set; }
        public string Message { get; set; }
        public string ImageLink { get; set; }
        [Required(ErrorMessage = "File Required")]
        public string File { get; set; }
    }
}