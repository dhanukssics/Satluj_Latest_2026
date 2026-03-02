using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbAcademicPeriod
{
    public long Id { get; set; }

    public long ClassId { get; set; }

    public long SchoolId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string PeriodsName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime TimeStamp { get; set; }

    public long RegionId { get; set; }

    public virtual TbClass Class { get; set; } = null!;

    public virtual TbRegions Region { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual ICollection<TbAttendancePeriod> TbAttendancePeriods { get; set; } = new List<TbAttendancePeriod>();

    public virtual ICollection<TbHealth> TbHealths { get; set; } = new List<TbHealth>();

    public virtual ICollection<TbVAssesment> TbVAssesments { get; set; } = new List<TbVAssesment>();
}
