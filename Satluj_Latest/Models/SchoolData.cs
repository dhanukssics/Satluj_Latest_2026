using Satluj_Latest.Data;

namespace Satluj_Latest.Models
{
    public class SchoolData
    {
        public School Data { get; set; }   
        public ClassDivisionValue value { get; set; }
    }

    public class ClassDivisionValue
    {
        public long classId { get; set; }
        public long divId { get; set; }
        public long schoolid { get; set; }
    }
}
