using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Satluj_Latest.Models.Temp;

[PrimaryKey("ConversationId", "ChatUserId")]
[Table("tb_ChatParticipants")]
[Index("ChatUserId", Name = "IX_ChatParticipants_User")]
public partial class TbChatParticipant
{
    [Key]
    public int ConversationId { get; set; }

    [Key]
    public int ChatUserId { get; set; }

    public DateTime JoinedOn { get; set; }

    public bool IsAdmin { get; set; }

    [ForeignKey("ChatUserId")]
    [InverseProperty("TbChatParticipants")]
    public virtual TbChatUser ChatUser { get; set; } = null!;

    [ForeignKey("ConversationId")]
    [InverseProperty("TbChatParticipants")]
    public virtual TbChatConversation Conversation { get; set; } = null!;
}
