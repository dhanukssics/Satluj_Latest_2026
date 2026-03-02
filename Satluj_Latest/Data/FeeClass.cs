using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class FeeClass : BaseReference
    {
        private TbFeeClass feeClass;
        public FeeClass(TbFeeClass obj) { feeClass = obj; }
        public FeeClass(long id) { feeClass = _Entities.TbFeeClasses.FirstOrDefault(z => z.FeeClassId == id); }
        public FeeClass(Guid Gid) { feeClass = _Entities.TbFeeClasses.FirstOrDefault(z => z.FeeClassGuid == Gid); }

        public long FeeClassId { get { return feeClass.FeeClassId; } }
        public long FeeId { get { return feeClass.FeeId; } }
        public decimal Amount { get { return feeClass.Amount; } }
        public long ClassId { get { return feeClass.ClassId; } }
        public System.DateTime Timestamp { get { return feeClass.TimeStamp; } }
        public System.DateTime DueDate { get { return feeClass.DueDate; } }

        public bool PublishStatus { get { return feeClass.PublishStatus; } }
        public bool IsActive { get { return feeClass.IsActive; } }
        public Fee FeeDetail { get { return new Fee(feeClass.Fee); } }
        public Class ClassDetail { get { return new Class(feeClass.ClassId); } }


    }
}
