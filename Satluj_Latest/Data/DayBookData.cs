using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class DayBookData : BaseReference
    {
        private TbDayBookDatum dayBookData;
        public DayBookData(TbDayBookDatum obj) { dayBookData = obj; }
        public DayBookData(long id) { dayBookData = _Entities.TbDayBookData.FirstOrDefault(z => z.DayBookId == id); }
        public long DayBookId { get { return dayBookData.DayBookId; } }
        public int TypeId { get { return dayBookData.TypeId; } }
        public System.DateTime EntryDate { get { return dayBookData.EntryDate; } }
        public string VoucherNo { get { return dayBookData.VoucherNo; } }
        public long HeadId { get { return dayBookData.HeadId; } }
        public long SubLedgerId { get { return dayBookData.SubLedgerId; } }
        public decimal Amount { get { return dayBookData.Amount; } }
        public string Narration { get { return dayBookData.Narration; } }
        public long SchoolId { get { return dayBookData.SchoolId; } }
        public long UserId { get { return dayBookData.UserId; } }
        public bool IsActive { get { return dayBookData.IsActive; } }
        public System.DateTime TimeStamp { get { return dayBookData.TimeStamp; } }
    }
}
