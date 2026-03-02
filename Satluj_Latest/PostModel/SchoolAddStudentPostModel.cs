using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class SchoolAddStudentPostModel
    {
        public string schoolId { get; set; }
        public string filePath { get; set; }
        public string studentName { get; set; }
        public string parentName { get; set; }
        public string address { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string contact { get; set; }
        public string classId { get; set; }
        public string divisionId { get; set; }
        public string busId { get; set; }
        public string classNo { get; set; }
        public string tripNo { get; set; }
        public string image { get; set; }
        public string admissionId { get; set; }
        public string gender { get; set; }
        public string bloodGroup { get; set; }
        public DateTime DOB { get; set; }
    }
}
