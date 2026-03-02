using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace Satluj_Latest.Models
{
    public class AssetsLiabilityModel
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Please Select any  type !")]
        public AssetsLiabilityType TypeId { get; set; }
        public DateTime EntryDate { get; set; }
        [Required(ErrorMessage = " Entry Date Required")]
        public string EntryDateString { get; set; }
        [Required(ErrorMessage = "Invice Number Required")]
        public string InviceNumber { get; set; }
        [Required(ErrorMessage = "Select any Account Head")]
        public long HeadId { get; set; }
        [Required(ErrorMessage = "Amount Required")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid Amount")]
        public decimal Amount { get; set; }
        public bool AddStatus { get; set; }
        public long SchoolId { get; set; }
        public long UserId { get; set; }
        public string Narration { get; set; }
        public string TypeData { get; set; }
        [Required(ErrorMessage = "Invice Number Required ... ")]
        public string SearchInviceNumber { get; set; }
        public string HeadName { get; set; }
        public string AddStatusString { get; set; }
    }
}