using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class ProgressCardModel
    {
        public long SchoolId { get; set; }
        public long RegionId { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long ExamId { get; set; }
        public string ProgressCardName { get; set; }
        public long StudentId { get; set; }
        public string ClassName { get; set; }
        public string RegionName { get; set; }
        public string DivisionName { get; set; }
        public string StudnetName { get; set; }
        public string Academicyear { get; set; }
        public List<SelectListItem> RegionList { get; set; }
            public List<SelectListItem> AcademicYearList { get; set; }

    }
}