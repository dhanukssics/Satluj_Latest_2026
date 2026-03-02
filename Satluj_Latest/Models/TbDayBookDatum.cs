using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbDayBookDatum
{
    public long DayBookId { get; set; }

    public int TypeId { get; set; }

    public int ExpenseId { get; set; }
    public int IncomeId { get; set; }

    public DateTime EntryDate { get; set; }

    public string VoucherNo { get; set; } = null!;

    public long HeadId { get; set; }

    public long SubLedgerId { get; set; }

    public decimal Amount { get; set; }

    public string Narration { get; set; } = null!;

    public long SchoolId { get; set; }

    public long UserId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool? IsWithdraw { get; set; }

    public string? VoucherType { get; set; }

    public string? TransactionType { get; set; }

    public virtual TbAccountHead Head { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbSubLedgerDatum SubLedger { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
