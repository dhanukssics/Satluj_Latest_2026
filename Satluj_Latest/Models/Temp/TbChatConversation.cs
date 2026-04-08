using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Satluj_Latest.Models.Temp;

[Table("tb_ChatConversation")]
public partial class TbChatConversation
{
    [Key]
    public int ConversationId { get; set; }

    public bool IsGroup { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? ClassId { get; set; }

    public int? DivisionId { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("TbChatConversations")]
    public virtual TbChatUser CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Conversation")]
    public virtual ICollection<TbChatMessage> TbChatMessages { get; set; } = new List<TbChatMessage>();

    [InverseProperty("Conversation")]
    public virtual ICollection<TbChatParticipant> TbChatParticipants { get; set; } = new List<TbChatParticipant>();
}
