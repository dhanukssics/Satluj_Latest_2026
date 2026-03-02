using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class SchoolRegistrationMapModel
    {
        public long SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Website { get; set; }
        public string Contact { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public System.Guid SchoolGuidId { get; set; }
        public bool IsActive { get; set; }
        public string State { get; set; }
        public string FilePath { get; set; }
        public SchoolLoginData Login { get; set; }
    }
}
