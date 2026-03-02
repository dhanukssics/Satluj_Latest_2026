using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Satluj_Latest.PostModel
{
    public class MessageFromSchoolPostModel
    {
        public class ClassDetails
        {
            public string ClassId { get; set; }
            public string DivisionId { get; set; }
        }

        public class MessageDetailsFromSchoolPostModel
        {
            public int TypeId { get; set; }
            public string Message { get; set; }
            public string SchoolId { get; set; }
            public List<ClassDetails> MultipleClass { get; set; }
        }
    }
}
