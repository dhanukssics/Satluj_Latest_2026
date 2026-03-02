using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSmsHistory
{
    public TbSmsHistory()
    {
    }

    public TbSmsHistory(TbSmsHistory x)
    {
        X = x;
    }

    public long Id { get; set; }

    public long StuentId { get; set; }

    public string? MessageContent { get; set; }

    public string? DelivaryStatus { get; set; }

    public DateTime? MessageDate { get; set; }

    public bool? IsActive { get; set; }

    public string? SendStatus { get; set; }

    public long ScholId { get; set; }

    public string? MobileNumber { get; set; }

    public int? SmsSentPerStudent { get; set; }

    public long? HeadId { get; set; }

    public string? MessageReturnId { get; set; }

    public virtual TbSmsHead? Head { get; set; }

    public virtual TbSchool Schol { get; set; } = null!;

    public virtual TbStudent Stuent { get; set; } = null!;
    public TbSmsHistory X { get; }
}
