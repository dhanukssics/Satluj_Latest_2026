using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class SubModuleModel:BaseReference
    {
        private TbSubModule subMo;
        public SubModuleModel(TbSubModule obj) { subMo = obj; }
        public SubModuleModel(long Id) { subMo = _Entities.TbSubModules.FirstOrDefault(z => z.Id == Id); }
        public long MessageId { get { return subMo.Id; } }
        public long MainId { get { return subMo.MainId; } }
        public string SUbModule { get { return subMo.SubModule; } }
        public bool IsActive { get { return subMo.IsActive; } }
        public System.DateTime TimeStamp { get { return subMo.TimeStamp; } }
        public string MainName { get { return subMo.Main.MainModule; } }

    }
}
