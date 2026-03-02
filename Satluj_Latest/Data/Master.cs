using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    class Master : BaseReference
    {
        private TbMaster master;
        public Master(TbMaster obj) { master = obj; }
        public Master(long MasterId) { master = _Entities.TbMasters.FirstOrDefault(z => z.MasterId == MasterId); }
        public long MasterId { get { return master.MasterId; } }
        public string MasterName { get { return master.MasterName; } }
        public long SchoolId { get { return master.SchoolId; } }
        public string ContactNo { get { return master.ContactNo; } }
        public string Address { get { return master.Address; } }
        public string PersonalInfo { get { return master.PersonalInfo; } }
        public string FilePath { get { return master.FilePath; } }
        public long UserId { get { return master.UserId; } }
        public bool IsActive { get { return master.IsActive; } }
        public System.DateTime TimeStamp { get { return master.TimeStamp; } }
    }
}
