using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class SubModule:BaseReference
    {
        private TbSubModule sub;
        public SubModule(TbSubModule obj) { sub = obj; }
        public SubModule(long id) { sub = _Entities.TbSubModules.FirstOrDefault(z => z.Id == id); }
        public long Id { get { return sub.Id; } }
        public long MainId { get { return sub.MainId; } }
        public string SUbModule { get { return sub.SubModule; } }
        public bool IsActive { get { return sub.IsActive; } }
        public System.DateTime TimeStamp { get { return sub.TimeStamp; } }
        public string ModuleName { get { return sub.Main.MainModule; } }
    }
}
