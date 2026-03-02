using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Satluj_Latest.Models
{
    public class AddDayBookModel
    {

        [Required(ErrorMessage = "Please Select any account type !")]
        public AccountType TypeId { get; set; }
        //[Required(ErrorMessage = "Day Book Entry Date Required")]
        [Required(ErrorMessage = "Voucher Number Required")]
        public string VoucherNo { get; set; }
        [Required(ErrorMessage = "Select any Account Head")]
        public long HeadId { get; set; }
        [Required(ErrorMessage = "Select a SubLedger")]
        public long SubLedgerId { get; set; }
        [Required(ErrorMessage = "Amount Required")]
        //[RegularExpression("^[0-9]*$", ErrorMessage = "Enter valid Amount")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Amount")]
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public long SchoolId { get; set; }
        public long UserId { get; set; }
        [Required(ErrorMessage = "Voucher Number Required ... ")]
        public string SearchVoucherNo { get; set; }
        public long DayBookId { get; set; }
        [Required(ErrorMessage = "Day Book Entry Date Required")]
        public string EntryDateString { get; set; }
        public DateTime EntryDate { get; set; }
        public string HeadName { get; set; }
        public string SubLedger { get; set; }
        public string TypeData { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string startTestDate { get; set; }
        public string endTestDate { get; set; }
        public int FilterTypeId { get; set; }
        public List<LedgerDataModel> LedgerList { get; set; }

    }
}