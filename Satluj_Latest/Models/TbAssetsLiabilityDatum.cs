using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbAssetsLiabilityDatum
{
    public long Id { get; set; }

    public int TypeId { get; set; }

    public DateTime EntryDate { get; set; }

    public string InviceNumber { get; set; } = null!;

    public long HeadId { get; set; }

    public decimal Amount { get; set; }

    public bool AddStatus { get; set; }

    public long SchoolId { get; set; }

    public long UserId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public string? Narration { get; set; }

    public virtual TbAccountHead Head { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
