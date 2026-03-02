using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Banks : BaseReference
    {
        private TbBank banks;
        public Banks(TbBank obj) { banks = obj; }
        public Banks(long BankId) { banks = _Entities.TbBanks.FirstOrDefault(z => z.BankId == BankId); }
        public long BankId { get { return banks.BankId; } }
        public string BankName { get { return banks.BankName; } }
        public long SchoolId { get { return banks.SchoolId; } }
        public bool IsActive { get { return banks.IsActive; } }
        public System.DateTime TimeStamp { get { return banks.TimeStamp; } }
    }
}
