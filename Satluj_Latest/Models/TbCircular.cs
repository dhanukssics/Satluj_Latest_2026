using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbCircular
{
    public TbCircular()
    {
    }

    public TbCircular(TbCircular x)
    {
        X = x;
    }

    public long CircularId { get; set; }

    public long SchoolId { get; set; }

    public int LoginType { get; set; }

    public long UserId { get; set; }

    public DateTime CircularDate { get; set; }

    public string Description { get; set; } = null!;

    public string? FilePath { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public string? CircularHead { get; set; }

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
    public TbCircular X { get; }
}
