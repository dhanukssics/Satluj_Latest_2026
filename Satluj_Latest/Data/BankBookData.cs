using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class BankBookData : BaseReference
    {
        private TbBankBookDatum bankBookData;
        public BankBookData(TbBankBookDatum obj) { bankBookData = obj; }
        public BankBookData(long Id) { bankBookData = _Entities.TbBankBookData.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return bankBookData.Id; } }
        public int TypeId { get { return bankBookData.TypeId; } }
        public DateTime EntryDate { get { return bankBookData.EntryDate; } }
        public string VoucherNumber { get { return bankBookData.VoucherNumber; } }
        public long BankId { get { return bankBookData.BankId; } }
        public long HeadId { get { return bankBookData.HeadId; } }
        public long SubledgerId { get { return bankBookData.SubledgerId; } }
        public decimal Amount { get { return bankBookData.Amount; } }
        public string ChequeNo { get { return bankBookData.ChequeNo; } }
        public DateTime? ChequeDate { get { return bankBookData.ChequeDate; } }
        public string Narration { get { return bankBookData.Narration; } }
        public long SchoolId { get { return bankBookData.SchoolId; } }
        public long UserId { get { return bankBookData.UserId; } }
        public bool IsActive { get { return bankBookData.IsActive; } }
        public DateTime TimeStamp { get { return bankBookData.TimeStamp; } }
        public string BankName { get { return bankBookData.Bank.BankName; } }
    }
}
