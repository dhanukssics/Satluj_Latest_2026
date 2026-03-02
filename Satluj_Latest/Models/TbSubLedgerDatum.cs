using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbSubLedgerDatum
{
    public TbSubLedgerDatum()
    {

    }
    public TbSubLedgerDatum(TbSubLedgerDatum z)
    {
        Z = z;
    }

    public long LedgerId { get; set; }

    public string SubLedgerName { get; set; } = null!;

    public long AccHeadId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbAccountHead AccHead { get; set; } = null!;

    public virtual ICollection<TbBankBookDatum> TbBankBookData { get; set; } = new List<TbBankBookDatum>();

    public virtual ICollection<TbDayBookDatum> TbDayBookData { get; set; } = new List<TbDayBookDatum>();
    public TbSubLedgerDatum Z { get; }
}
