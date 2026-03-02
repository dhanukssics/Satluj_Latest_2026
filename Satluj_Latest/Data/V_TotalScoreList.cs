using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class V_TotalScoreList : BaseReference
    {
        private TbVTotalScoreList vTS;
        public V_TotalScoreList(TbVTotalScoreList obj) { vTS = obj; }
        public V_TotalScoreList(long id) { vTS = _Entities.TbVTotalScoreLists.FirstOrDefault(z => z.Id == id); }

        
        public long Id { get { return vTS.Id; } }
        public long SchoolId { get { return vTS.SchoolId; } }
        public bool IsSubject { get { return vTS.IsSubject; } }
        public int EnumTypeId { get { return vTS.EnumTypeId; } }
        public string SubTitle { get { return vTS.SubTitle; } }
        public long Mark { get { return vTS.Mark; } }
        public bool IsActive { get { return vTS.IsActive; } }
        public System.DateTime TimeStamp { get { return vTS.TimeStamp; } }

        public TbVTotalScoreList X { get; }
    }
}
