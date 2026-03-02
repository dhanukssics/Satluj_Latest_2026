using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class FeeAlertDataModel
    {
        public long Id { get; set; }
        public long SchoolId { get; set; }
        [Required(ErrorMessage = "Exam Date Required")]
        public DateTime AlertDate { get; set; }
        public string AlertDateString { get; set; }
        public bool IsActive { get; set; }
        public DateTime? TimeStamp { get; set; }
        [Required(ErrorMessage = "Exam Date Required")]
         public string FooterMessage { get; set; }
        public int libraryDueDay { get; set; }
        [Required(ErrorMessage = "Required")]
        public long feeId { get; set; }
        [Required(ErrorMessage = "Required")]
        public decimal libFineAmount { get; set; }
        public SchoolSenderIdModel SenderDetails { get; set; }
        public FeeIncomeAccountHead FeeIncomeHead { get; set; } //***

    }
    public class SchoolSenderIdModel
    {
        public long SenderId { get; set; }
        public string SenderData { get; set; }
    }
    public class FeeIncomeAccountHead //***
    {
        public long HeadId { get; set; }
        public string AccountHead { get; set; }
    }
}