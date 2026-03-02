using System;
using System.Collections.Generic;

namespace Satluj_Latest.Models;

public partial class TbAttendancePeriod
{
    public long Id { get; set; }

    public long SchoolId { get; set; }

    public long StudentId { get; set; }

    public long DivisionId { get; set; }

    public long PeriodId { get; set; }

    public long UserId { get; set; }

    public int TotalDays { get; set; }

    public int PresentDays { get; set; }

    public bool IsActive { get; set; }

    public DateTime TimeStampp { get; set; }

    public virtual TbDivision Division { get; set; } = null!;

    public virtual TbAcademicPeriod Period { get; set; } = null!;

    public virtual TbSchool School { get; set; } = null!;

    public virtual TbStudent Student { get; set; } = null!;

    public virtual TbLogin User { get; set; } = null!;
}
