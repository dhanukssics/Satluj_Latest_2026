using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbCcavenueCourseResponse
{
    public long ResponseId { get; set; }

    public long? ParentId { get; set; }

    public string? OrderId { get; set; }

    public string? TrackingId { get; set; }

    public bool? OrderStatus { get; set; }

    public string? PaymentMode { get; set; }

    public string? Course { get; set; }

    public decimal? Amount { get; set; }

    public long? BillNo { get; set; }

    public long? SchoolId { get; set; }

    public virtual TbParent? Parent { get; set; }

    public virtual TbSchool? School { get; set; }
}
