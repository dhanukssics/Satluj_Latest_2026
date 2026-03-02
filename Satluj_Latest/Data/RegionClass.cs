using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
   public  class RegionClass:BaseReference
    {
        private TbRegionsClass regClass;
        public RegionClass(TbRegionsClass obj) { regClass = obj; }
        public RegionClass(long Id) { regClass = _Entities.TbRegionsClasses.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return regClass.Id; } }
        public long RegionId { get { return regClass.RegionId; } }
        public string RegionName { get { return regClass.Region.RegionName; } }
        public long ClassId { get { return regClass.ClassId; } }
        public string ClassName { get { return regClass.Class.Class; } }
        public bool IsActive { get { return regClass.IsActive; } }
        public System.DateTime TimeStamp { get { return regClass.TimeStamp; } }
    }
}
