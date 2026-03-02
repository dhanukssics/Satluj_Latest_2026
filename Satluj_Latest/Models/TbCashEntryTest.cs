using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbCashEntryTest
{
    public decimal Id { get; set; }

    public decimal VoucherNumber { get; set; }

    public string VoucherType { get; set; } = null!;

    public decimal? BillNo { get; set; }

    public string TransactionType { get; set; } = null!;

    public decimal? Amount { get; set; }

    public decimal HeadId { get; set; }

    public decimal SubId { get; set; }

    public string Narration { get; set; } = null!;

    public DateTime EnterDate { get; set; }

    public decimal UserId { get; set; }

    public decimal DataFromStatus { get; set; }

    public decimal CancelStatus { get; set; }

    public decimal SchoolId { get; set; }

    public decimal Migration { get; set; }

    public decimal IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public string? EditStatus { get; set; }

    public decimal? ReverseStatus { get; set; }

    public decimal? AdvanceStatus { get; set; }
}
