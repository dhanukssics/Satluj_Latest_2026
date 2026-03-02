using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class SchoolRegisterModel
    {
        [Required(ErrorMessage = "Required")]
        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid")]
        public string schoolName { get; set; }

        [Required(ErrorMessage = "Required")]
        //[RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Invalid")]
        public string address { get; set; }

        [Required(ErrorMessage = "Required")]
        public string state { get; set; }

        [Required(ErrorMessage = "Required")]
        //[RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid")]
        public string city { get; set; }

        //[Required(ErrorMessage = "Required")]
        public string website { get; set; }

        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Entered e-mail is not a valid mail")]
        public string emailaddress { get; set; }

        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Not a valid number")]
        [StringLength(10, ErrorMessage = "Contact Number Should be Maximum 10 digit")]
        public string contactNumber { get; set; }

        [Required(ErrorMessage = "Required")]
        public string password { get; set; }
        [Required(ErrorMessage = "Required")]
        [Compare("password", ErrorMessage = "Password is not matching")]
        public string confirmpassword { get; set; }
        public string image { get; set; }
        public string FilePath { get; set; }
        public bool ValidateForm { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You gotta tick the box!")]
        public bool tearms { get; set; }

    }
}