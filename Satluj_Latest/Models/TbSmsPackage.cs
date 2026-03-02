using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSmsPackage
{
    public long PackageId { get; set; }

    public long SchoolId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public long AllowedSms { get; set; }

    public decimal SmsRate { get; set; }

    public bool IsActive { get; set; }

    public bool IsDisabled { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
