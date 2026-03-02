using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSmsHead
{
    public long HeadId { get; set; }

    public string Head { get; set; } = null!;

    public long SchoolId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public int? SenderType { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbHomeworkSm> TbHomeworkSms { get; set; } = new List<TbHomeworkSm>();

    public virtual ICollection<TbSmsHistory> TbSmsHistories { get; set; } = new List<TbSmsHistory>();

    public virtual ICollection<TbStaffSmshistory> TbStaffSmshistories { get; set; } = new List<TbStaffSmshistory>();
}
