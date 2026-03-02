using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class ScolasticAreaResultDetails:BaseReference
    {
        private TbScolasticAreaResultDetail sarDetails;
        public ScolasticAreaResultDetails(TbScolasticAreaResultDetail obj) { sarDetails = obj; }
        public ScolasticAreaResultDetails(long Id) { sarDetails = _Entities.TbScolasticAreaResultDetails.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return sarDetails.Id; } }
        public long MainId { get { return sarDetails.MainId; } }
        public long SubjectId { get { return sarDetails.SubjectId; } }
        public decimal Score { get { return sarDetails.Score; } }
        public bool IsActive { get { return sarDetails.IsActive; } }
        public System.DateTime TimeStamp { get { return sarDetails.TimeStamp; } }
    }
}
