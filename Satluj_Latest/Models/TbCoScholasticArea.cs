using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbCoScholasticArea
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long RegionId { get; set; }

    public string ItemName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public long? ClassId { get; set; }

    public int? OrderNo { get; set; }

    public virtual TbClass? Class { get; set; }

    public virtual TbRegions Region { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbCoScholasticResultmain> TbCoScholasticResultmains { get; set; } = new List<TbCoScholasticResultmain>();
}
