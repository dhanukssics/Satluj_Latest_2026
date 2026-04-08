namespace Satluj_Latest.Models
{
    public class ChatListVM
    {
        public int ConversationId { get; set; }
        public string ChatTitle { get; set; }
        public string SubTitle { get; set; }
        public string LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public bool IsGroup { get; set; }
        public int? OtherChatUserId { get; set; }
    }
}
