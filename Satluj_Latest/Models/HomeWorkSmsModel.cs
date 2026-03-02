using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Satluj_Latest.Models
{
    public class HomeWorkSmsModel
    {

        public long SchoolId { get; set; }
        public long SubjectId { get; set; }
        public string Numbers { get; set; }
        public string StudentId { get; set; }
        public string Description { get; set; }
        public string Data { get; set; }
        public int MessageSentPerStudent { get; set; }

    }
}