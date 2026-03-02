using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbDeletedFeeStudent
{
    public long DletedFeeId { get; set; }

    public long FeeClassId { get; set; }

    public long StudentId { get; set; }

    public Guid ParentGuid { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbFeeClass FeeClass { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
