using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Co_ScholasticResultmain:BaseReference
    {
        private TbCoScholasticResultmain csrMain;
        public Co_ScholasticResultmain(TbCoScholasticResultmain obj) { csrMain = obj; }
        public Co_ScholasticResultmain(long Id) { csrMain = _Entities.TbCoScholasticResultmains.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return csrMain.Id; } }
        public long SchoolId { get { return csrMain.SchoolId; } }
        public long StudentId { get { return csrMain.StudentId; } }
        public long ExamId { get { return csrMain.ExamId; } }
        public long Co_ScholasticId { get { return csrMain.CoScholasticId; } }
        public string Score { get { return csrMain.Score; } }
        public bool IsActive { get { return csrMain.IsActive; } }
        public System.DateTime TimeStamp { get { return csrMain.TimeStamp; } }
    }
}
