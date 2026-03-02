using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class PasswordChangeModel
    {
        [Required(ErrorMessage = "Required")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Required")]
        public string Newpassword { get; set; }

        [Required(ErrorMessage = "Required")]
        [CompareAttribute("Newpassword", ErrorMessage = "Password doesn't match.")]
        public string ConfirmPassword { get; set; }
      

        public long SchoolId { get; set; }
    }
}