using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbFeeClass
{
    private Guid feeGuid;

    public TbFeeClass()
    {
    }

    public TbFeeClass(Guid feeGuid)
    {
        this.feeGuid = feeGuid;
    }

    public long FeeClassId { get; set; }

    public long FeeId { get; set; }

    public decimal Amount { get; set; }

    public long ClassId { get; set; }

    public Guid FeeClassGuid { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool PublishStatus { get; set; }

    public bool IsActive { get; set; }

    public DateTime DueDate { get; set; }

    public int Instalment { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbFee Fee { get; set; } = null!;

    public virtual ICollection<TbDeletedFeeStudent> TbDeletedFeeStudents { get; set; } = new List<TbDeletedFeeStudent>();
}
