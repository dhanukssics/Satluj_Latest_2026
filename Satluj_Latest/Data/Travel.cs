using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class Travel : BaseReference
    {
        private TbTravel travel;
        public Travel(TbTravel obj) { travel = obj; }
        public Travel(long id) { travel = _Entities.TbTravels.FirstOrDefault(z => z.TravelId == id); }
        public long TravelId { get { return travel.TravelId; } }
        public long TripId { get { return travel.TripId; } }
        public string Longitude { get { return travel.Longitude; } }
        public string Latitude { get { return travel.Latitude; } }
        public string Place { get { return travel.Place; } }
        public bool IsActive { get { return travel.IsActive; } }
        public System.DateTime TimeStamp { get { return travel.TimeStamp; } }
        public string TimeStampString { get { return travel.TimeStamp.ToString("HH:mm:ss tt"); } }
        public System.Guid TravelGuid { get { return travel.TravelGuid; } }
    }
}
