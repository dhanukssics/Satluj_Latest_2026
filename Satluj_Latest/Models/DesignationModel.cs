using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class DesignationModel
    {
        public long SchoolId { get; set; }
        public long Id { get; set; }
        [Required(ErrorMessage = "Designation Name Required")]
        public string DesignationName { get; set; }
        public string DesignationCode { get; set; }
    }
}