using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
   public  class Expence:BaseReference
    {
        private TbExpense expn;
        public Expence(TbExpense obj) { expn = obj; }
        public Expence(long id) { expn = _Entities.TbExpenses.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return expn.Id; } }
        public string AccountHead { get { return expn.AccountHead; } }
        public string Particular { get { return expn.Particular; } }
        public Nullable<double> Amount { get { return expn.Amount; } }
        public bool IsActive { get { return expn.IsActive; } }
        public long SchoolId { get { return expn.SchoolId; } }
    
    }

}
