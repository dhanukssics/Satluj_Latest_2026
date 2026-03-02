using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbScholasticArea
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long RegionId { get; set; }

    public string ItemName { get; set; } = null!;

    public decimal TotalScore { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public decimal? DividedBy { get; set; }

    public int? SpecificTerm { get; set; }

    public virtual TbRegions Region { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbScholasticResultMain> TbScholasticResultMains { get; set; } = new List<TbScholasticResultMain>();
}
