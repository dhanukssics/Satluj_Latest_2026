using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbPayment
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

    public int? PaymentMode { get; set; }

    public string? ChequeNumber { get; set; }

    public DateTime? ChequeDate { get; set; }

    public long? BankId { get; set; }

    public long? PartialPaidParentBillNo { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbFee Fee { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
