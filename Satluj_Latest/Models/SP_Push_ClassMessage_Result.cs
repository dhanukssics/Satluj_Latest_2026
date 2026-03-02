

namespace Satluj_Latest.Models
{
    public partial class SP_Push_ClassMessage_Result
    {
        public SP_Push_ClassMessage_Result()
        {
            
        }
        public long TokenId { get; set; }
        public int RoleId { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public bool IsActive { get; set; }
        public int LoginStatus { get; set; }
    }
}
