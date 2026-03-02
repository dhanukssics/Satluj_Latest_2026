using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
   public  class Income:BaseReference
    {
        private TbIncome income;
        public Income(TbIncome obj) { income = obj; }
        public Income(long id) { income = _Entities.TbIncomes.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return income.Id; } }

        public long SchoolId { get { return income.SchoolId; } }
        public string AccountHead { get { return income.AccountHead; } }
        public string Particular { get { return income.Particular; } }
        public Nullable<double> Amount { get { return income.Amount; } }
        public bool IsActive { get { return income.IsActive; } }
    }
}
