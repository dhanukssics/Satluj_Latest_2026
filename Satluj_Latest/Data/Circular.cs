using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Circular : BaseReference
    {
        private TbCircular circular;
        public Circular(TbCircular obj) { circular = obj; }
        public Circular(long id) { circular = _Entities.TbCirculars.FirstOrDefault(z => z.CircularId == id); }
        public long CircularId { get { return circular.CircularId; } }
        public long SchoolId { get { return circular.SchoolId; } }
        public int LoginType { get { return circular.LoginType; } }
        public long USerId { get { return circular.UserId; } }
        public DateTime CircularDate { get { return circular.CircularDate; } }
        public string Description { get { return circular.Description; } }
        public string FilePath { get { return circular.FilePath; } }
        public bool IsActive { get { return circular.IsActive; } }
        public DateTime TimeStamp { get { return circular.TimeStamp; } }
        public string CircularHead { get{ return circular.CircularHead; } }
    }
}
