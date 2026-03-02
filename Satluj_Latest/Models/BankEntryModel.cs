using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Satluj_Latest.Models
{
    public class BankEntryModel
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
        //[Required(ErrorMessage = "Narration Required ... ")]
        public string Narration { get; set; }
        public long SchoolId { get; set; }
        public long UserId { get; set; }
        [Required(ErrorMessage = "Voucher Number Required ... ")]
        public string SearchVoucherNo { get; set; }
        public long BankBookId { get; set; }
        [Required(ErrorMessage = "Bank Book Entry Date Required")]
        public string EntryDateString { get; set; }
        public DateTime EntryDate { get; set; }
        public string VoucherType { get; set; }
        public string TransactionType { get; set; }
        public string HeadName { get; set; }
        public string SubLedger { get; set; }
        public string TypeData { get; set; }
        //[Required(ErrorMessage = "Cheque Number Required ... ")]
        public string ChequeNo { get; set; }
        public string ChequeDateString { get; set; }
        public DateTime ChequeDate { get; set; }
        public string BankName { get; set; }
        public decimal BalanceAmount { get; set; }
        public bool CashTransaction { get; set; }
        public PaymentMode PaymentModeId { get; set; }
        public int bankTypeId { get; set; }
        public int contra { get; set; }
        public string voucher { get; set; }
        public string SubLedgerData { get; set; }
        public long BankDataId { get; set; }
    }
}