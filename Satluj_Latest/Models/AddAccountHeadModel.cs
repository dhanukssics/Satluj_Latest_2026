using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class AddAccountHeadModel
    {

        [Required(ErrorMessage = "Account Head Required")]
        public string AccountHeadName { get; set; }
        public long SchoolId { get; set; }
        [Required(ErrorMessage = "Select any Account head !")]
        public long AccountHeadId { get; set; }
        [Required(ErrorMessage = "Sub Ledger Required")]
        public string SubLedger { get; set; }
    }
}