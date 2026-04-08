namespace Satluj_Latest.Models
{
    public class ChatsPageVM
    {
        public List<ChatListVM> AllChats { get; set; }
        public List<ChatListVM> GroupChats { get; set; }
        public List<ChatListVM> ClassChats { get; set; }
        public List<TeacherClassVM> TeacherClasses { get; set; }
    }
}
