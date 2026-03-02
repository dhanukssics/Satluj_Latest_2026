using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbRegionsClass
{
    public long Id { get; set; }

    public long RegionId { get; set; }

    public long ClassId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbRegions Region { get; set; } = null!;
}
