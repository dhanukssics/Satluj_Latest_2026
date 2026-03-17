using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class FeeModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "0 not allowed")]
        [Required(ErrorMessage = "Required")]
        public decimal Amount { get; set; }
        public string FeeDetails { get; set; }
        public string FeeStudentId { get; set; }
        public string StudentName { get; set; }
        [Required(ErrorMessage = "Required")]
        public string FeeName { get; set; }
        public long SpecialFeeId { get; set; }

        public long BillNumber { get; set; }

        public long DivisionId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PaidAmount { get; set; }


        public long BankId { get; set; }
        public long SchoolId { get; set; }
        public long ClassId { get; set; }
        public long StudentId { get; set; }
        public long FeeId { get; set; }
        public string ChequeDate { get; set; }
        public string ChequeNumber { get; set; }
        public int PaymentType { get; set; }


        public DateTime TimeStamp { get; set; }
        public SchoolModel SchoolModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int From { get; set; } // 12-12-2018 Archana 
        public long BillNo { get; set; }// 12-12-2018 Archana 
        public List<TbStudent> Students { get; internal set; }
    }
}