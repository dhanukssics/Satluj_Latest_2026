using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class SchoolLoginMapModel
    {
        public long SchoolId { get; set; }
        public string SchoolName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Website { get; set; }
        public string Contact { get; set; }
        public bool IsActive { get; set; }
        public string State { get; set; }
        public string FilePath { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public SchoolLoginData Login { get; set; }
    }
}
