using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbFeeDiscount
{
    public TbFeeDiscount()
    {
    }

    public TbFeeDiscount(TbFeeDiscount q)
    {
        Q = q;
    }

    public long DiscountId { get; set; }

    public long StudentId { get; set; }

    public long FeeId { get; set; }

    public decimal DiscountAmount { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool IsActive { get; set; }

    public virtual TbFee Fee { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
    public TbFeeDiscount Q { get; }
}
