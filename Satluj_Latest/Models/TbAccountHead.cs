using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbAccountHead
{
    public long AccountId { get; set; }

    public string AccHeadName { get; set; } = null!;

    public long SchoolId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool? ForBill { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbAssetsLiabilityDatum> TbAssetsLiabilityData { get; set; } = new List<TbAssetsLiabilityDatum>();

    public virtual ICollection<TbBankBookDatum> TbBankBookData { get; set; } = new List<TbBankBookDatum>();

    public virtual ICollection<TbBankEntry> TbBankEntries { get; set; } = new List<TbBankEntry>();

    public virtual ICollection<TbCashEntry> TbCashEntries { get; set; } = new List<TbCashEntry>();

    public virtual ICollection<TbDayBookDatum> TbDayBookData { get; set; } = new List<TbDayBookDatum>();

    public virtual ICollection<TbSubLedgerDatum> TbSubLedgerData { get; set; } = new List<TbSubLedgerDatum>();
}
