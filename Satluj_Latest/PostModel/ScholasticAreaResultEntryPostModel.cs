using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class ScholasticAreaResultEntryPostModel
    {
        public long ExamId { get; set; }
        public string ExamName { get; set; }
        public List<ScholasticList> _ScholasticList { get; set; }
    }
    public class ScholasticList
    {
        public long ScholasticId { get; set; }
        public string ScholasticArea { get; set; }
        public List<SubjectList> _SubjectList { get; set; }
    }
    public class SubjectList
    {
        public long SubjectId { get; set; }
        public string SubjectName { get; set; }
    }


}
