using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class ScholasticAreaResultMapModel
    {
        public string SchoolId { get; set; }
        public string ExamId { get; set; }
        public string ScholasticId { get; set; }
        public string SubjectId { get; set; }
        public List<StudentScholasticResult> _StudentScholasticResult { get; set; }
    }
    public class StudentScholasticResult
    {
        public string StudentId { get; set; }
        public string Mark { get; set; }
    }
}
