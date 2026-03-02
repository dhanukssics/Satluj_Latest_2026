using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public  class DriverTripCompletePostModel
    {
        public string tripId { get; set; }
        public string completeTime { get; set; }

        public string latitude { get; set; }
        public string longitude  { get; set; }

    }
}
