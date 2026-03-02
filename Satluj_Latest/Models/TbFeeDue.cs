using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbFeeDue
{
    public long FeeDuesId { get; set; }

    public decimal Amount { get; set; }

    public long FeeId { get; set; }

    public long StudentId { get; set; }

    public Guid FeeDuesGuid { get; set; }

    public bool IsActive { get; set; }

    public DateTime DueDate { get; set; }

    public DateTime TimeStamp { get; set; }

    public Guid? ParentGuid { get; set; }

    public long? BillNo { get; set; }

    public virtual TbFee Fee { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
