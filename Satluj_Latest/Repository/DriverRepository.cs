using Satluj_Latest.PostModel;
using Satluj_Latest.Data;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Satluj_Latest.Repository
{
    public class DriverRepository
    {
        private readonly SchoolDbContext _Entity;
        //public tb_Satluj_LatestEntities _Entity = new tb_Satluj_LatestEntities();
        public DateTime currentTime = DateTime.UtcNow;
        public Tuple<bool, string, Driver> DriverLogin(DriverLoginPostModel model)
        {
            var status = false;
            var msg = "Failed";
            var driverData = _Entity.TbDrivers.Where(x => x.DriverSpecialId == model.driverSpecialId && x.IsActive).ToList().Select(z => new Driver(z)).FirstOrDefault();
            if (driverData != null)
            {
                msg = "Success";
                status = true;
                return new Tuple<bool, string, Driver>(status, msg, driverData);
            }
            else
            {
                msg = "Invalid DriverId";
                return new Tuple<bool, string, Driver>(status, msg, null);
            }
        }

        public Tuple<bool, string, Bus> BusIdentification(DriverBusIdentificationPostModel model)
        {
            string msg = "Failed";
            bool status = false;
            long schoolId = Convert.ToInt64(model.schoolId);
            int tripNo = Convert.ToInt32(model.tripNo);
            var busDetails = _Entity.TbBus.Where(x => x.BusSpecialId == model.busSpecialId && x.TripNumber >= tripNo && x.SchoolId == schoolId && x.IsActive).ToList().Select(z => new Bus(z)).FirstOrDefault();
            if (busDetails != null)
            {
                status = true;
                msg = "Success";
                return new Tuple<bool, string, Bus>(status, msg, busDetails);
            }
            else
            {
                msg = "No such Bus";
                return new Tuple<bool, string, Bus>(status, msg, null);
            }
        }

        public Tuple<bool, string, long> TripStart(DriverTripStartPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long schoolId = Convert.ToInt64(model.schoolId);
            long driverId = Convert.ToInt64(model.driverId);
            long busId = Convert.ToInt64(model.busId);
            int shiftStatus = Convert.ToInt32(model.shiftStatus);
            DateTime tripDate = Convert.ToDateTime(model.tripDate);
            //var data = Getdatata(model.latitude, model.longitude); 22-11-2018 Google API issue commend the line
            //if (data != "no location found")
            {
                //var trip = _Entity.TbTrips.Create();
                var trip = new TbTrip();
                trip.DriverId = driverId;
                trip.SchoolId = schoolId;
                trip.TripNo = model.tripNo;
                trip.TripDate = tripDate;
                trip.FromLocation = model.startPlace;
                trip.ToLocation = model.endPlace;
                trip.StartTime = tripDate;
                trip.ReachTime = tripDate;
                trip.TimeStamp = currentTime;
                trip.TripGuid = Guid.NewGuid();
                trip.IsActive = true;
                trip.TravellingStatus = 0;
                trip.BusId = busId;
                trip.ShiftStatus = shiftStatus;
                _Entity.TbTrips.Add(trip);
                status = _Entity.SaveChanges() > 0;
                if (status == true)
                {
                    //var travel = _Entity.TbTravels.Create();
                    var travel = new TbTravel();
                    travel.TripId = trip.TripId;
                    travel.Latitude = model.latitude;
                    travel.Longitude = model.longitude;
                    //travel.Place = data.ToString();
                    travel.Place = "";
                    travel.IsActive = true;
                    travel.TimeStamp = currentTime;
                    travel.TravelGuid = Guid.NewGuid();
                    _Entity.TbTravels.Add(travel);
                    status = _Entity.SaveChanges() > 0;
                    msg = "Success";
                }
                else
                {
                    msg = "Error Occured";
                }
                return new Tuple<bool, string, long>(status, msg, trip.TripId);
            }
            //else
            //{
            //    status = false;
            //    msg = "GPS not found";
            //    return new Tuple<bool, string, long>(status, msg,0);
            //}

        }

        public Tuple<bool, string> Travelling(DriverTravellingPostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long tripId = Convert.ToInt64(model.tripId);
            var tripData = _Entity.TbTrips.Where(x => x.TripId == tripId).FirstOrDefault();
            tripData.TravellingStatus = 1;
            status = _Entity.SaveChanges() > 0;
            //if (status == true)
            //{
            var data = Getdatata(model.latitude, model.longitude);
            if (data != null)
            {
                //var travel = _Entity.tb_Travel.Create()
                var travel = new TbTravel();
                travel.TripId = tripData.TripId;
                travel.Latitude = model.latitude;
                travel.Longitude = model.longitude;
                travel.Place = data.ToString();
                travel.IsActive = true;
                travel.TimeStamp = currentTime;
                travel.TravelGuid = Guid.NewGuid();
                _Entity.TbTravels.Add(travel);
                status = _Entity.SaveChanges() > 0;
                msg = "Success";
            }
            //}
            return new Tuple<bool, string>(status, msg);
        }

        public Tuple<bool, string> TripComplete(DriverTripCompletePostModel model)
        {
            bool status = false;
            string msg = "Failed";
            long tripId = Convert.ToInt64(model.tripId);
            DateTime completeDate = Convert.ToDateTime(model.completeTime);
            var tripData = _Entity.TbTrips.Where(x => x.TripId == tripId && x.IsActive).FirstOrDefault();
            tripData.TravellingStatus = 2;
            tripData.ReachTime = completeDate;
            status = _Entity.SaveChanges() > 0;
            if (status == true)
            {
                var data = Getdatata(model.latitude, model.longitude);
                if (data != null)
                {
                    //var travel = _Entity.tb_Travel.Create();
                    var travel = new TbTravel();
                    travel.TripId = tripData.TripId;
                    travel.Latitude = model.latitude;
                    travel.Longitude = model.longitude;
                    travel.Place = data.ToString();
                    travel.IsActive = true;
                    travel.TimeStamp = currentTime;
                    travel.TravelGuid = Guid.NewGuid();
                    _Entity.TbTravels.Add(travel);
                    status = _Entity.SaveChanges() > 0;
                    msg = "Success";
                }
            }
            return new Tuple<bool, string>(status, msg);
        }

        private string Getdatata(string lat, string longd)
        {
            string location = "";
            string url = "http://maps.google.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
            url = string.Format(url, lat, longd);
            WebRequest request = WebRequest.Create(url);
            try
            {
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        DataSet dsResult = new DataSet();
                        dsResult.ReadXml(reader);
                        var x = 1;

                        location = dsResult.Tables["result"].Rows[0]["formatted_address"].ToString();
                        return location;
                    }
                }
            }
            catch (Exception ex)
            {
                location = "no location found";
                return location;
            }

        }
    }
}
