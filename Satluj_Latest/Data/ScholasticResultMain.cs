using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ScholasticResultMain:BaseReference
    {
        private TbScholasticResultMain scResult;
        public ScholasticResultMain(TbScholasticResultMain obj) { scResult = obj; }
        public ScholasticResultMain(long Id) { scResult = _Entities.TbScholasticResultMains.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return scResult.Id; } }
        public long SchoolId { get { return scResult.SchoolId; } }
        public long StudentId { get { return scResult.StudentId; } }
        public long ExamId { get { return scResult.ExamId; } }
        public long ScholasticId { get { return scResult.ScholasticId; } }
        public bool IsActive { get { return scResult.IsActive; } }
        public System.DateTime TimeStamp { get { return scResult.TimeStamp; } }
    }
}
