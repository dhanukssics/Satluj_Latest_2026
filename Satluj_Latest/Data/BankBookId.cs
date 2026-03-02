using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class BankBookId:BaseReference
    {
        private TbBankBookId bankBookId;
        public BankBookId(TbBankBookId obj) { bankBookId = obj; }
        public BankBookId(long Id) { bankBookId = _Entities.TbBankBookIds.FirstOrDefault(z => z.Id == Id); }
        public long SchoolId { get { return bankBookId.SchoolId; } }
        public long DepositId { get { return bankBookId.DepositId; } }
        public long WithdrawId { get { return bankBookId.WithdrawId; } }
    }
}
