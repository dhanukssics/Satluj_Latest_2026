using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbParentMessage
{
    public long MessageId { get; set; }

    public long SenderId { get; set; }

    public long StudentId { get; set; }

    public string? Subject { get; set; }

    public string? Description { get; set; }

    public string? FilePath { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool ReadStatus { get; set; }

    public virtual TbParent Sender { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
