using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ParentRegisterModel
    {
        public string type { get; set; }
        public long schoolId { get; set; }
        public long classId { get; set; }
        public long studentId { get; set; }
        public long parentId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string parentName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string address { get; set; }
        [Required(ErrorMessage = "Required")]
        public string city { get; set; }
        [Required(ErrorMessage = "Required")]
        [EmailAddress(ErrorMessage = "Entered e-mail is not a valid mail")]
        public string email { get; set; }
        [Required(ErrorMessage = "Required")]
        public string password { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Not a valid number")]
        [StringLength(10, ErrorMessage = "Contact Number Should be Maximum 10 digit")]
        public string contactNo { get; set; }
        [Required(ErrorMessage = "Required")]
        public string postalCode { get; set; }
        [Required(ErrorMessage = "Required")]
        public string state { get; set; }
        public string image { get; set; }
        public string FilePath { get; set; }
    }
}