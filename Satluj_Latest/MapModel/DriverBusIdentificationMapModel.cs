using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class DriverBusIdentificationMapModel
    {
        public long BusId { get; set; }
        public string BusSpecialId { get; set; }
        public string TripNumber { get; set; }
        public string LocationStart { get; set; }
        public string LocationEnd { get; set; }
        public System.Guid BusGuid { get; set; }
        public bool IsActive { get; set; }
        public string BusType { get; set; }
        public string BusName { get; set; }
        public long SchoolId { get; set; }
    }
}
