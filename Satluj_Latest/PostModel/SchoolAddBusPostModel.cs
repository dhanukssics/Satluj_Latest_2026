using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class SchoolAddBusPostModel
    {
        public string schoolId { get; set; }
        public string busName { get; set; }
        public string tripNumber { get; set; }
        //public string startLocation { get; set; }
        public string endLocation { get; set; }
        public string busType { get; set; }
    }
}
