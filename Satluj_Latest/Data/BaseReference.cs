using Microsoft.EntityFrameworkCore;
using Satluj_Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.Data
{
    public class BaseReference
    {
        //protected tb_Satluj_LatestEntities _Entities = new tb_Satluj_LatestEntities();
        protected SchoolDbContext _Entities = new SchoolDbContext();

        public DateTime CurrentTime = TimeZoneInfo.ConvertTimeFromUtc(System.DateTime.Now.ToUniversalTime(), TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

    }
}
