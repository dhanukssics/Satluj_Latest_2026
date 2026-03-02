namespace Satluj_Latest.Models
{
    public class UpPublishedClassList
    {
        public List<UpPublishedClassItem> list { get; set; } = new();
    }

    public class UpPublishedClassItem
    {
        public int SlNo { get; set; }
        public long ClassId { get; set; }
        public long DivisionId { get; set; }
        public string ClassName { get; set; }
        public string DivisionName { get; set; }
        public string AccademicYear { get; set; }
        public bool CurrentYearStatus { get; internal set; }
    }
}
