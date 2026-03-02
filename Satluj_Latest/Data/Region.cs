using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
   public  class Region:BaseReference
    {
        private TbRegions reg;
        public Region(TbRegions obj) { reg = obj; }
        public Region(long Id) { reg = _Entities.TbRegionss.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return reg.Id; } }
        public long SchoolId { get { return reg.SchoolId; } }
        public string RegionName { get { return reg.RegionName; } }
        public bool IsActive { get { return reg.IsActive; } }
        public System.DateTime TimeStamp { get { return reg.TimeStamp; } }
        public List<RegionClass> RegionClassData()
        {
            return reg.TbRegionsClasses.Where(x => x.IsActive).Select(x => new RegionClass(x)).ToList();
        }
    }
}
