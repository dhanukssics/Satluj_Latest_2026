using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbHealth
{
    public long Id { get; set; }

    public long ClassId { get; set; }

    public long DivisionId { get; set; }

    public long PeriodsId { get; set; }

    public long StudentId { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbAcademicPeriod Periods { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;
}
