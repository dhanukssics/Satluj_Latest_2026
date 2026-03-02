using System;
using System.ComponentModel.DataAnnotations;

namespace Satluj_Latest.Models
{
    public class EditFeeClass
    {
        public long feeClassId { get; set; }
        public long feeId { get; set; }
        public int? FeeType { get; set; }
        public string FeeName { get; set; }

        public string classname { get; set; }

        [Required]
        public decimal amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }
    }
}
