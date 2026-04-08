using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Satluj_Latest.Models.Temp;

[Table("tb_ChatUsers")]
public partial class TbChatUser
{
    [Key]
    public int ChatUserId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string UserType { get; set; } = null!;

    [StringLength(50)]
    public string RefId { get; set; } = null!;

    [StringLength(100)]
    public string DisplayName { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public bool? IsOnline { get; set; }

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<TbChatConversation> TbChatConversations { get; set; } = new List<TbChatConversation>();

    [InverseProperty("FromChatUser")]
    public virtual ICollection<TbChatMessage> TbChatMessages { get; set; } = new List<TbChatMessage>();

    [InverseProperty("ChatUser")]
    public virtual ICollection<TbChatParticipant> TbChatParticipants { get; set; } = new List<TbChatParticipant>();
}
