using Satluj_Latest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class CoScholasticAreaModel
    {
        public long SchoolId { get; set; }
        public long RegionId { get; set; }
        public string Item { get; set; }
        public long Id { get; set; }
        public string RegionName { get; set; }
        public List<Co_ScholasticArea> CoScholasticAreaList { get; set; }
    }
}