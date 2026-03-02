using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Fee : BaseReference
    {
        private readonly TbFee _fee;
        public Fee(TbFee obj)
        {
            _fee = obj ?? throw new ArgumentNullException(nameof(obj));
        }
        public Fee(long id)
        {
            _fee = _Entities.TbFees.FirstOrDefault(z => z.FeeId == id)
                  ?? throw new Exception($"Fee record with ID {id} not found.");
        }

        public long FeeId => _fee.FeeId;
        public int? FeeType => _fee.FeeType;
        public string FeeName => _fee.FeesName;
        public long SchoolId => _fee.SchoolId;
        public DateTime Timestamp => _fee.TimeStamp;
        public bool IsActive => _fee.IsActive;
        public DateTime? DueDate => _fee.DueDate;
        public decimal? FineAmount => _fee.FineAmount;
        public int? NoOfDays => _fee.NoOfDays;
    }
}
