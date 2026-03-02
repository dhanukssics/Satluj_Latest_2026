namespace Satluj_Latest.Models
{
    public class SpParentTeacherConversationFullResult
    {
        public long MessageId { get; set; }
        public long SenderId { get; set; }
        public long StudentId { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public string Role { get; set; }
        public int Status { get; set; }
    }
}
