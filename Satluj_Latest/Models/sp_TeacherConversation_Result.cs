namespace Satluj_Latest.Models
{
    public partial class sp_TeacherConversation_Result
    {
        public sp_TeacherConversation_Result()
        {
            
        }
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
