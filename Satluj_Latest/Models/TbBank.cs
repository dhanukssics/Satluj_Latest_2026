using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbBank
{
    public TbBank()
    {

    }
    public TbBank(TbBank z)
    {
        Z = z;
    }

    public long BankId { get; set; }

    public string BankName { get; set; } = null!;

    public long SchoolId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbBankBookDatum> TbBankBookData { get; set; } = new List<TbBankBookDatum>();

    public virtual ICollection<TbBankEntry> TbBankEntries { get; set; } = new List<TbBankEntry>();
    public TbBank Z { get; }
}
