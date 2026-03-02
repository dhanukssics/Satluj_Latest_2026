using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStaffSmshistory
{
    public long Id { get; set; }

    public long StaffId { get; set; }

    public string? MessageContent { get; set; }

    public string? DelivaryStatus { get; set; }

    public DateTime? MessageDate { get; set; }

    public bool IsActive { get; set; }

    public string? SendStatus { get; set; }

    public long ScholId { get; set; }

    public string? MobileNumber { get; set; }

    public int? SmsSentPerStudent { get; set; }

    public long HeadId { get; set; }

    public int? UserType { get; set; }

    public string? MessageReturnId { get; set; }

    public virtual TbSmsHead Head { get; set; } = null!;

    public virtual TbSchool Schol { get; set; } = null!;

    public virtual TbLogin Staff { get; set; } = null!;
}
