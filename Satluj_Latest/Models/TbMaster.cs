using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbMaster
{
    public long MasterId { get; set; }

    public string MasterName { get; set; } = null!;

    public long SchoolId { get; set; }

    public string ContactNo { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? PersonalInfo { get; set; }

    public string? FilePath { get; set; }

    public long UserId { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
