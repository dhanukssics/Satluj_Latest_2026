using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbBankBookDatum
{
    public long Id { get; set; }

    public int TypeId { get; set; }

    public DateTime EntryDate { get; set; }

    public string VoucherNumber { get; set; } = null!;

    public long BankId { get; set; }

    public long HeadId { get; set; }

    public long SubledgerId { get; set; }

    public decimal Amount { get; set; }

    public string? ChequeNo { get; set; }

    public DateTime? ChequeDate { get; set; }

    public string Narration { get; set; } = null!;

    public long SchoolId { get; set; }

    public long UserId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool? IsWithdraw { get; set; }

    public string? VoucherType { get; set; }

    public string? TransactionType { get; set; }

    public virtual TbBank Bank { get; set; } = null!;

    public virtual TbAccountHead Head { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbSubLedgerDatum Subledger { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
