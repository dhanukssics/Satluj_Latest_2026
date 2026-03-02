using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class RemarkClass:BaseReference
    {
        private TbRemark rk;
        public RemarkClass(TbRemark obj) { rk = obj; }
        public RemarkClass(long Id) { rk = _Entities.TbRemarks.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return rk.Id; } }
        public long SchoolId { get { return rk.SchoolId; } }
        public string Remark { get { return rk.Remark; } }
        public bool IsActive { get { return rk.IsActive; } }
        public System.DateTime Timestamp { get { return rk.Timestamp; } }

    }
}
