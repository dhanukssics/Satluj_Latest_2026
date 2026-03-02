using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ChangePasswordModel
    {
        public System.Guid LoginGuid { get; set; }

        [Required(ErrorMessage = "Required")]
        //[StringLength(int.MaxValue, MinimumLength = 7)]
        [StringLength(15, MinimumLength = 7, ErrorMessage = "Password must be between 7 and 15")]
        public string password { get; set; }
        
        //[Compare("password")]
        [CompareAttribute("password", ErrorMessage = "Password doesn't match.")]
        [Required(ErrorMessage = "Required")]
        public string confirmPassword { get; set; }
    }
}