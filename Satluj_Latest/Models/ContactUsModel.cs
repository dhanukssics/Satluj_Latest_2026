using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ContactUsModel
    {
        [Required(ErrorMessage = "Name Required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Email Required")]
        //[EmailAddress(ErrorMessage = "Email not valid")]
        public string email { get; set; }
        [Required(ErrorMessage = "Contact Number Required")]
        public string contactNo { get; set; }
        [Required(ErrorMessage = "School Name Required")]
        public string schoolName { get; set; }
        [Required(ErrorMessage = "Message Required")]
        public string message { get; set; }
    }
}