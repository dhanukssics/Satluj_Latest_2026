using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Driver:BaseReference
    {
         private TbDriver driver;
         public Driver(TbDriver obj) { driver = obj; }
         public Driver(long id) { driver = _Entities.TbDrivers.FirstOrDefault(z => z.DriverId == id); }
         public long DriverId { get { return driver.DriverId; } }
         public long SchoolId { get { return driver.SchoolId; } }
         public string DriverSpecialId { get { return driver.DriverSpecialId; } }
         public string DriverName { get { return driver.DriverName; } }
         public string LicenseNumber { get { return driver.LicenseNumber; } }
         public string ContactNumber { get { return driver.ContactNumber; } }
         public string Address { get { return driver.Address; } }
         public System.DateTime TimeStamp { get { return driver.TimeStamp; } }
         public System.Guid DriverGuid { get { return driver.DriverGuid; } }
         public bool IsActive { get { return driver.IsActive; } }
         public string FilePath { get { return driver.FilePath; } }
         public string City { get { return driver.City; } }
         public string State { get { return driver.State; } }
         public School School { get { return new School(driver.School); } }
    }
}
