using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Satluj_Latest.Models.Temp;

[Table("tb_ChatMessages")]
[Index("ConversationId", "CreatedOn", Name = "IX_ChatMessages_Conversation")]
public partial class TbChatMessage
{
    [Key]
    public int MessageId { get; set; }

    public int ConversationId { get; set; }

    public int FromChatUserId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public bool IsRead { get; set; }

    [ForeignKey("ConversationId")]
    [InverseProperty("TbChatMessages")]
    public virtual TbChatConversation Conversation { get; set; } = null!;

    [ForeignKey("FromChatUserId")]
    [InverseProperty("TbChatMessages")]
    public virtual TbChatUser FromChatUser { get; set; } = null!;
}
