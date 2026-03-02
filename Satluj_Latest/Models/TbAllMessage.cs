using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbAllMessage
{
    public long MessageId { get; set; }

    public long TeacherId { get; set; }

    public long ToMsgSentId { get; set; }

    public string Subject { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int MessageType { get; set; }

    public string Filepath { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual TbTeacher Teacher { get; set; } = null!;
}
