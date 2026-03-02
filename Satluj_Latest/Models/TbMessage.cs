using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbMessage
{
    public long MessageId { get; set; }

    public long TeacherId { get; set; }

    public long StudentId { get; set; }

    public string? Subject { get; set; }

    public string? Descrption { get; set; }

    public bool MessageType { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool IsActive { get; set; }

    public virtual TbStudent Student { get; set; } = null!;

    public virtual TbTeacher Teacher { get; set; } = null!;
}
