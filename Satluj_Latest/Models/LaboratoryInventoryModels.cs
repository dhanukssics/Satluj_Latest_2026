using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class LaboratoryInventoryModels
    {
        public long categoryId { get; set; }
        public string laboratoryName { get; set; }
        public long schoolId { get; set; }
        public bool isActive { get; set; }
    }
}

