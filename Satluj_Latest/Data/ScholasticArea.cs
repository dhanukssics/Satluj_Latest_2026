using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ScholasticArea :BaseReference
    {
        private TbScholasticArea sArea;
        public ScholasticArea(TbScholasticArea obj) { sArea = obj; }
        public ScholasticArea(long Id) { sArea = _Entities.TbScholasticAreas.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return sArea.Id; } }
        public long SchoolId { get { return sArea.SchoolId; } }
        public long RegionId { get { return sArea.RegionId; } }
        public string RegionName { get { return sArea.Region.RegionName; } }
        public string ItemName { get { return sArea.ItemName; } }
        public decimal TotalScore { get { return sArea.TotalScore; } }
        public bool IsActive { get { return sArea.IsActive; } }
        public System.DateTime TimeStamp { get { return sArea.TimeStamp; } }
    }
}
