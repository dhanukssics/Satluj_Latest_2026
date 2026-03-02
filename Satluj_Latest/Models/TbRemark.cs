using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbRemark
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public string Remark { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;
}
