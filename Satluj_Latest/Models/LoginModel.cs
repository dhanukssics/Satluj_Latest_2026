using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class LoginModel
    {
        public long SchoolId { get; set; }
        public long UserId { get; set; }
        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Entered e-mail is not a valid mail")]
        public string Email { get; set; }
        //[StringLength(10, MinimumLength = 6, ErrorMessage = "Password must be in between 6 and 10")]
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
        public int userType { get; set; }
        public string type { get; set; }
    }
}