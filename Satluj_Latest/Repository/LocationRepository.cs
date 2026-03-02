using Satluj_Latest.Data;
using Satluj_Latest.Models;
using Satluj_Latest.PostModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.DataLibrary.Repository
{
    public class LocationRepository
    {
        //public tb_Satluj_LatestEntities _Entity = new tb_Satluj_LatestEntities();
        private readonly SchoolDbContext _Entity;

        public DateTime currentTime = DateTime.UtcNow;
        public Tuple<bool, string, Travel> tracklocation(TrackStudentLocationPostModel model)
        {
            var status = true;
            string msg = "success";
            string busSpecialId = model.busSpecialId;
            var bus = _Entity.TbBus.Where(x => x.BusSpecialId == busSpecialId && x.IsActive == true).FirstOrDefault();
            string tripNo = model.tripNo;
            DateTime todayNow = currentTime;
            var tripData = _Entity.TbTrips.Where(x => x.BusId == bus.BusId && x.TripNo == tripNo && x.IsActive && x.StartTime >= currentTime).FirstOrDefault();
            var travelData = _Entity.TbTravels.Where(x => x.TripId == tripData.TripId).OrderByDescending(z => z.TravelId).ToList().Select(z=>new Travel(z)).FirstOrDefault();
            return new Tuple<bool, string, Travel>(status, msg, travelData);
        }
    }
}
