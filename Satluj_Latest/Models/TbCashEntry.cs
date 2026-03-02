using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbCashEntry
{
    public long Id { get; set; }

    public string VoucherNumber { get; set; } = null!;

    public string VoucherType { get; set; } = null!;

    public string? BillNo { get; set; }

    public string TransactionType { get; set; } = null!;

    public decimal? Amount { get; set; }

    public long HeadId { get; set; }

    public long SubId { get; set; }

    public string Narration { get; set; } = null!;

    public DateTime EnterDate { get; set; }

    public long UserId { get; set; }

    public bool DataFromStatus { get; set; }

    public bool CancelStatus { get; set; }

    public long SchoolId { get; set; }

    public bool Migration { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public string? EditStatus { get; set; }

    public bool? ReverseStatus { get; set; }

    public bool? AdvanceStatus { get; set; }

    public virtual TbAccountHead Head { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
