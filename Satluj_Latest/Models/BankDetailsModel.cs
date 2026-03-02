using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class BankDetailsModel
    {
        public long BankId { get; set; }

        public string BankName { get; set; }
        [Required(ErrorMessage = "Bank Name Required")]
        public long SchoolId { get; set; }
    }
}