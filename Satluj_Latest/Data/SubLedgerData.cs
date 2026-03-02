using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
  public  class SubLedgerData: BaseReference
    {
        private TbSubLedgerDatum subLedgerData;
        public SubLedgerData(TbSubLedgerDatum obj) { subLedgerData = obj; }
        public SubLedgerData(long id) { subLedgerData = _Entities.TbSubLedgerData.FirstOrDefault(z => z.LedgerId == id); }
        public long LedgerId { get { return subLedgerData.LedgerId; } }
        public string SubLedgerName { get { return subLedgerData.SubLedgerName; } }
        public long AccHeadId { get { return subLedgerData.AccHeadId; } }
        public bool IsActive { get { return subLedgerData.IsActive; } }
        public System.DateTime TimeStamp { get { return subLedgerData.TimeStamp; } }
        public string AccountHeadName { get { return subLedgerData.AccHead.AccHeadName; } }

       
    }
}
