using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbState
{
    public long StateId { get; set; }

    public string State { get; set; } = null!;

    public Guid StateGuid { get; set; }

    public bool IsActive { get; set; }
}
