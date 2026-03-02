using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbAssetsLiabilityId
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long AssetsId { get; set; }

    public long LiabilityId { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
