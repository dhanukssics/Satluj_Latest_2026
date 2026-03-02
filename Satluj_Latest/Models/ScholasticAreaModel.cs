using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ScholasticAreaModel
    {
        public long RegionId { get; set; }
        public long SchoolId { get; set; }
        public string AreaName { get; set; }
        public decimal Score { get; set; }
        public long Id { get; set; }
        public string RegionName { get; set; }
        public List<SelectListItem> RegionList { get; set; }

    }
}