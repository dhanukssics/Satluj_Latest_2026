using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
   public class DriverLoginMapModel
    {
       public long DriverId { get; set; }
       public long SchoolId { get; set; }
       public string DriverSpecialId { get; set; }
       public string DriverName { get; set; }
       public string LicenseNumber { get; set; }
       public string ContactNumber { get; set; }
       public string Address { get; set; }
       public System.Guid DriverGuid { get; set; }
       public string FilePath { get; set; }
       public string City { get; set; }
       public string State { get; set; }
       public SchoolMapData School { get; set; }
    }
}
