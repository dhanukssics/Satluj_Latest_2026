using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbPaymentTest
{
    public long PaymentId { get; set; }

    public Guid FeeGuid { get; set; }

    public decimal Amount { get; set; }

    public bool IsPaid { get; set; }

    public long FeeId { get; set; }

    public long StudentId { get; set; }

    public long ClassId { get; set; }

    public long SchoolId { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool IsActive { get; set; }

    public decimal? MaxAmount { get; set; }

    public decimal? Discount { get; set; }

    public Guid? PaymentGuid { get; set; }

    public long? BillNo { get; set; }

    public long PaymentType { get; set; }

    public int? BillType { get; set; }
}
