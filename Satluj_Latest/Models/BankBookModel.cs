using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Satluj_Latest.Models
{
    public class BankBookModel
    {
        [Required(ErrorMessage = "Please Select any account type !")]
        public BankType TypeId { get; set; }
        [Required(ErrorMessage = "Voucher Number Required")]
        public string VoucherNo { get; set; }
        [Required(ErrorMessage = "Select any Account Head")]
        public long HeadId { get; set; }
        [Required(ErrorMessage = "Select a SubLedger")]
        public long SubLedgerId { get; set; }
        [Required(ErrorMessage = "Select a Bank")]
        public long BankId { get; set; }

        [Required(ErrorMessage = "Amount Required")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Amount")]
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public long SchoolId { get; set; }
        public long UserId { get; set; }
        [Required(ErrorMessage = "Voucher Number Required ... ")]
        public string SearchVoucherNo { get; set; }
        public long BankBookId { get; set; }
        [Required(ErrorMessage = "Bank Book Entry Date Required")]
        public string EntryDateString { get; set; }
        public DateTime EntryDate { get; set; }
        public string HeadName { get; set; }
        public string SubLedger { get; set; }
        public string TypeData { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeDateString { get; set; }
        public DateTime ChequeDate { get; set; }
        public string BankName { get; set; }
        public decimal BalanceAmount { get; set; }
        public bool iswithdraw { get; set; }

    }
}