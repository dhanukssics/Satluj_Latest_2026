using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
   public class CertificateName: BaseReference
    {
        private TbCertificateName name;
        public CertificateName(TbCertificateName obj) { name = obj; }
        public CertificateName(long Id) { name = _Entities.TbCertificateNames.FirstOrDefault(z => z.Id == Id); }
        public long Id { get { return name.Id; } }
        public long SchoolId { get { return name.SchoolId; } }
        public string CertificateNames { get { return name.CertificateName; } }
        public bool IsActive { get { return name.IsActive; } }
        public System.DateTime TimeStamp { get { return name.TimeStamp; } }
    }
}
