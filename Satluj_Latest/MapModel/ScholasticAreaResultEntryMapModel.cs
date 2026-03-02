using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.MapModel
{
    public class ScholasticAreaResultEntryMapModel
    {
        public string SchoolId { get; set; }
        public string ClassId { get; set; }
        public string DivisionId { get; set; }
    }
    public class ScholasticAreaResultStudentsListMapModel
    {
        public string SchoolId { get; set; }
        public string ClassId { get; set; }
        public string DivisionId { get; set; }
        public string ExamId { get; set; }
        public string ScholasticId { get; set; }
        public string SubjectId { get; set; }
    }
}
