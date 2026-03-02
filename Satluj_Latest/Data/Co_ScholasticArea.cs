using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Co_ScholasticArea:BaseReference
    {
        private TbCoScholasticArea csArea;
        public Co_ScholasticArea(TbCoScholasticArea obj) { csArea = obj; }
        public Co_ScholasticArea(long Id) { csArea = _Entities.TbCoScholasticAreas.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return csArea.Id; } }
        public long SchoolId { get { return csArea.SchoolId; } }
        public long RegionId { get { return csArea.RegionId; } }
        public string RegionName { get { return csArea.Region.RegionName; } }
        public string ItemName { get { return csArea.ItemName; } }
        public bool IsActive { get { return csArea.IsActive; } }
        public System.DateTime TimeStamp { get { return csArea.TimeStamp; } }
    }
}
