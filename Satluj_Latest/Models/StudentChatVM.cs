namespace Satluj_Latest.Models
{
    public class StudentChatVM
    {
        public long StudentId { get; set; }
        public long? ParentId { get; set; }
        public string StudentName { get; set; }
        public string ParentName { get; set; }
    }
}
