using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class AccountHead:BaseReference
    {
        private TbAccountHead accountHead;
        public AccountHead(TbAccountHead obj) { accountHead = obj; }
        public AccountHead(long id) { accountHead = _Entities.TbAccountHeads.FirstOrDefault(z => z.AccountId == id); }
        public long AccountId { get { return accountHead.AccountId; } }
        public string AccHeadName { get { return accountHead.AccHeadName; } }
        public long SchoolId { get { return accountHead.SchoolId; } }
        public bool IsActive { get { return accountHead.IsActive; } }
        public System.DateTime TimeStamp { get { return accountHead.TimeStamp; } }

        
    }
}
