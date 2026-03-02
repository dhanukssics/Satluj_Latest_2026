using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class ParentAddKidMapModel
    {
        public long StudentId { get; set; }
        public long SchoolId { get; set; }
        public string StudentSpecialId { get; set; }
        public string StundentName { get; set; }
        public string ParentName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactNumber { get; set; }
        public string ClasssNumber { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public long BusId { get; set; }
        public string TripNo { get; set; }
        public string FilePath { get; set; }
        public System.Guid StudentGuid { get; set; }
        public string State { get; set; }
        public string Class { get; set; }
        public AddDivision Division { get; set; }
    }
}
