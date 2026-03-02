using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
   public class ParentRegistrationMapModel
    {
       public long ParentId { get; set; }
       public string ParentName { get; set; }
       public string Address { get; set; }
       public string City { get; set; }
       public string Email { get; set; }
       public string ContactNumber { get; set; }
       public System.Guid ParentGuid { get; set; }
       public bool IsActive { get; set; }
       public string State { get; set; }
       public string FilePath { get; set; }
    }
}
