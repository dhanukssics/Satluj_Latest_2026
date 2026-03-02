using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbStudentPremotion
{
    public long PremotionId { get; set; }

    public long StudentId { get; set; }

    public long FromDivision { get; set; }

    public long ToDivision { get; set; }

    public DateTime TimeStamp { get; set; }

    public long? SchoolId { get; set; }

    public long IsActive { get; set; }

    public bool? LastUpdate { get; set; }

    public long? OldClass { get; set; }

    public virtual TbDivision FromDivisionNavigation { get; set; } = null!;

    public virtual TbClass? OldClassNavigation { get; set; }

    public virtual TbSchool? School { get; set; }

    public virtual TbStudent Student { get; set; } = null!;

    public virtual TbDivision ToDivisionNavigation { get; set; } = null!;
}
