using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Satluj_Latest;

namespace Satluj_Latest.Models
{
    public class CashEntryModel
    {
        [Required(ErrorMessage = "Please Select any account type !")]
        public AccountType TypeId { get; set; }

        [Required(ErrorMessage = "Voucher Number Required")]
        public string VoucherNo { get; set; }
        [Required(ErrorMessage = "Select any Account Head")]
        public long HeadId { get; set; }
        [Required(ErrorMessage = "Select a SubLedger")]
        public long SubLedgerId { get; set; }
        [Required(ErrorMessage = "Amount Required")]

        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Amount")]
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public long SchoolId { get; set; }
        public long UserId { get; set; }
        [Required(ErrorMessage = "Voucher Number Required ... ")]
        public long CashEntryId { get; set; }
        [Required(ErrorMessage = "Day Book Entry Date Required")]
        public string EntryDateString { get; set; }
        public DateTime EntryDate { get; set; }
        public string VoucherType { get; set; }
        public string TransactionType { get; set; }
        public string HeadName { get; set; }
        public string SubLedger { get; set; }
        public string TypeData { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public bool ReverseStatus { get; set; }
        public bool AdvanceStatus { get; set; }
        public int cashTypeId { get; set; }
        public string voucher { get; set; }
        public string SubLedgerData { get; set; }
        public long CashId { get; set; }
    }
}