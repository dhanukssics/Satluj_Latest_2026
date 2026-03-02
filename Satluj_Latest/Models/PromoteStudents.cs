using Microsoft.AspNetCore.Mvc.Rendering;
using static Satluj_Latest.Models.SchoolValue;

namespace Satluj_Latest.Models
{
    public class PromoteStudents
    {
        public long SchoolId { get; set; }
        public long OldClassId { get; set; }
        public long OldDivId { get; set; }
        public long OldAcademicyearId { get; set; }
        public long NewCLassId { get; set; }
        public long NewDivId { get; set; }
        public long NewAcademicYearId { get; set; }
        public List<StudentListForPromote> StudentList { get; set; }
        public string StudentListString { get; set; }
        public List<SelectListItem> AcademicYearList { get; set; }

    }
}
