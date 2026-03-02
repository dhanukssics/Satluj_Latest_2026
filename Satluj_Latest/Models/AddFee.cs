using System;
using System.ComponentModel.DataAnnotations;

namespace Satluj_Latest.Models
{
    public class AddFee
    {
        public long SchoolId { get; set; }

        [Required]
        public string FeeName { get; set; }

        public int FeeType { get; set; }   // 1 = Common, 2 = Special

        public bool IsReccuring { get; set; }
        public bool IsDueDate { get; set; }

        public string EndDateString { get; set; }
        public string DueDateString { get; set; }

        public string HaveFineDateString { get; set; }
        public decimal FineAmount { get; set; }
        public int FineDays { get; set; }

        public int Interval { get; set; }
        public DateTime HaveFineDate { get; internal set; }
    }
}
