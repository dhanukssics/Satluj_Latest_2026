using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbLibraryFine
{
    public long Id { get; set; }

    public long FeeId { get; set; }

    public long SchoolId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public decimal FineAmount { get; set; }

    public virtual TbFee Fee { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;
}
