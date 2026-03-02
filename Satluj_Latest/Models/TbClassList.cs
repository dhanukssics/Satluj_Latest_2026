using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbClassList
{
    public long ClassNameId { get; set; }

    public string ClassName { get; set; } = null!;

    public int OrderValue { get; set; }

    public bool IsActive { get; set; }
}
